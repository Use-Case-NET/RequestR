// RequestR
// Copyright (C) 2021-2025 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Reflection;

namespace DustInTheWind.RequestR;

/// <summary>
/// This is the service that is used to execute use cases based on received requests.
/// </summary>
public class RequestBus
{
    private readonly UseCaseFactoryBase useCaseFactory;
    private readonly Dictionary<Type, Type> useCases = new Dictionary<Type, Type>();
    private readonly Dictionary<Type, Type> validators = new Dictionary<Type, Type>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestBus"/> class
    /// with a default implementation of the use case factory.
    /// </summary>
    public RequestBus()
    {
        useCaseFactory = new DefaultUseCaseFactory();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestBus"/> class
    /// with a custom use case factory.
    /// </summary>
    public RequestBus(UseCaseFactoryBase useCaseFactory)
    {
        this.useCaseFactory = useCaseFactory ?? throw new ArgumentNullException(nameof(useCaseFactory));
    }

    /// <summary>
    /// Adds a new use case type to the internal list of known use cases.
    /// Later, when a request is received, a use case type is peeked from this list, instantiated and executed.
    /// </summary>
    /// <typeparam name="TUseCase">The type of the use case to be added in the list.</typeparam>
    public void RegisterUseCase<TUseCase>()
    {
        RegisterUseCase(typeof(TUseCase));
    }

    /// <summary>
    /// Adds a new use case type to the internal list of known use cases.
    /// Later, when a request is received, a use case type is peeked from this list, instantiated and executed.
    /// </summary>
    /// <param name="useCaseType">The type of the use case to be added in the list.</param>
    public void RegisterUseCase(Type useCaseType)
    {
        Type interfaceType = useCaseType.GetUseCaseInterface();

        if (interfaceType == null)
            throw new TypeIsNotUseCaseException(useCaseType);

        Type requestType = interfaceType.GenericTypeArguments[0];

        if (useCases.ContainsKey(requestType))
            throw new UseCaseAlreadyRegisteredException(requestType);

        useCases.Add(requestType, useCaseType);
    }

    /// <summary>
    /// Adds a new validator type to the internal list of known validators.
    /// Later, when a request is received, the corresponding validator is peeked from the list, instantiated and executed.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator to be added in the list.</typeparam>
    public void RegisterValidator<TValidator>()
    {
        RegisterValidator(typeof(TValidator));
    }

    /// <summary>
    /// Adds a new validator type to the internal list of known validators.
    /// Later, when a request is received, the corresponding validator is peeked from the list, instantiated and executed.
    /// </summary>
    /// <param name="validatorType">The type of the validator to be added in the list.</param>
    public void RegisterValidator(Type validatorType)
    {
        Type interfaceType = validatorType.GetRequestValidatorInterface();

        if (interfaceType == null)
            throw new ArgumentException("The specified type is not a request validator. It must implement the interface " + typeof(IRequestValidator<>).FullName);

        Type requestType = interfaceType.GenericTypeArguments[0];

        if (validators.ContainsKey(requestType))
            throw new ValidatorAlreadyRegisteredException(requestType);

        validators.Add(requestType, validatorType);
    }

    /// <summary>
    /// Searches a use case that can handle the specified request, executes it and returns the response.
    /// If a validator exists for the request, it is also executed before the use case.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object for which to execute the use case.</typeparam>
    /// <typeparam name="TResponse">The type of the response object that is returned to the caller.</typeparam>
    /// <param name="request">The request object for which to execute the use case.</param>
    /// <returns>The response object that is returned to the caller.</returns>
    public TResponse Process<TRequest, TResponse>(TRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        ValidateRequest(request);
        object useCaseObject = InstantiateUseCaseFor<TRequest>();

        switch (useCaseObject)
        {
            case IUseCase<TRequest, TResponse> useCaseWithResponse:
                {
                    return useCaseWithResponse.Execute(request, CancellationToken.None).Result;
                }

            case IUseCase<TRequest> useCaseWithoutResponse:
                {
                    useCaseWithoutResponse.Execute(request, CancellationToken.None).Wait();
                    return default;
                }

            default:
                {
                    Type useCaseInterfaceType = useCaseObject.GetType().GetUseCaseInterface();

                    if (useCaseInterfaceType.GetGenericTypeDefinition() == typeof(IUseCase<,>))
                    {
                        MethodInfo executeMethodInfo = useCaseInterfaceType.GetMethods()
                            .Single(x => IsExecuteMethod(x, typeof(TRequest)));

                        Task task = (Task)executeMethodInfo.Invoke(useCaseObject, new object[] { request, CancellationToken.None });
                        task.Wait();

                        PropertyInfo resultProperty = task.GetType().GetProperty("Result");
                        object responseObject = resultProperty.GetValue(task);

                        try
                        {
                            return (TResponse)Convert.ChangeType(responseObject, typeof(TResponse));
                        }
                        catch (Exception ex)
                        {
                            Type actualResponseType = responseObject.GetType();
                            Type requestedResponseType = typeof(TResponse);

                            throw new ResponseCastException(actualResponseType, requestedResponseType, ex);
                        }
                    }

                    throw new UnusableUseCaseException(useCaseObject.GetType(), typeof(TRequest));
                }
        }
    }

    /// <summary>
    /// Searches a use case that can handle the specified request and executes it.
    /// If a validator exists for the request, it is also executed before the use case.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object for which to execute the use case.</typeparam>
    /// <param name="request">The request object for which to execute the use case.</param>
    public void Process<TRequest>(TRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        ValidateRequest(request);
        object useCaseObject = InstantiateUseCaseFor<TRequest>();

        if (useCaseObject is IUseCase<TRequest> useCaseWithoutResponse)
        {
            useCaseWithoutResponse.Execute(request, CancellationToken.None).Wait();
        }
        else
        {
            Type useCaseInterfaceType = useCaseObject.GetType().GetUseCaseInterface();

            if (useCaseInterfaceType.GetGenericTypeDefinition() == typeof(IUseCase<,>))
            {
                MethodInfo executeMethodInfo = useCaseInterfaceType.GetMethods()
                    .Single(x => IsExecuteMethod(x, typeof(TRequest)));

                Task task = (Task)executeMethodInfo.Invoke(useCaseObject, new object[] { request, CancellationToken.None });
                task.Wait();
            }
            else
            {
                throw new UnusableUseCaseException(useCaseObject.GetType(), typeof(TRequest));
            }
        }
    }

    private static bool IsExecuteMethod(MethodInfo methodInfo, Type requestType)
    {
        if (methodInfo.Name != "Execute")
            return false;

        ParameterInfo[] parameterInfos = methodInfo.GetParameters();

        if (parameterInfos.Length != 2)
            return false;

        if (parameterInfos[0].ParameterType != requestType)
            return false;

        return true;
    }

    /// <summary>
    /// Searches a use case that can handle the specified request, executes it asynchronously and returns the response.
    /// If a validator exists for the request, it is also executed before the use case.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object for which to execute the use case.</typeparam>
    /// <typeparam name="TResponse">The type of the response object that is returned to the caller.</typeparam>
    /// <param name="request">The request object for which to execute the use case.</param>
    /// <returns>
    /// The <see cref="Task"/> object representing the asynchronous execution.
    /// At the end of the execution, the <see cref="Task{T}.Result"/> will contain the response object of the use case.
    /// </returns>
    public Task<TResponse> ProcessAsync<TRequest, TResponse>(TRequest request)
    {
        return ProcessAsync<TRequest, TResponse>(request, CancellationToken.None);
    }

    /// <summary>
    /// Searches a use case that can handle the specified request, executes it asynchronously and returns the response.
    /// If a validator exists for the request, it is also executed before the use case.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object for which to execute the use case.</typeparam>
    /// <typeparam name="TResponse">The type of the response object that is returned to the caller.</typeparam>
    /// <param name="request">The request object for which to execute the use case.</param>
    /// <returns>
    /// The <see cref="Task"/> object representing the asynchronous execution.
    /// At the end of the execution, the <see cref="Task{T}.Result"/> will contain the response object of the use case.
    /// </returns>
    public async Task<TResponse> ProcessAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        ValidateRequest(request);
        object useCaseObject = InstantiateUseCaseFor<TRequest>();

        switch (useCaseObject)
        {
            case IUseCase<TRequest, TResponse> useCaseWithResponse:
                return await useCaseWithResponse.Execute(request, cancellationToken);

            case IUseCase<TRequest> useCaseWithoutResponse:
                await useCaseWithoutResponse.Execute(request, cancellationToken);
                return default;

            default:
                {
                    Type useCaseInterfaceType = useCaseObject.GetType().GetUseCaseInterface();

                    if (useCaseInterfaceType.GetGenericTypeDefinition() == typeof(IUseCase<,>))
                    {
                        MethodInfo executeMethodInfo = useCaseInterfaceType.GetMethods()
                            .Single(x => IsExecuteMethod(x, typeof(TRequest)));

                        Task task = (Task)executeMethodInfo.Invoke(useCaseObject, new object[] { request, cancellationToken });
                        await task;

                        PropertyInfo resultProperty = task.GetType().GetProperty("Result");
                        object responseObject = resultProperty.GetValue(task);

                        try
                        {
                            return (TResponse)Convert.ChangeType(responseObject, typeof(TResponse));
                        }
                        catch (Exception ex)
                        {
                            Type actualResponseType = responseObject.GetType();
                            Type requestedResponseType = typeof(TResponse);

                            throw new ResponseCastException(actualResponseType, requestedResponseType, ex);
                        }
                    }

                    throw new UnusableUseCaseException(useCaseObject.GetType(), typeof(TRequest));
                }
        }
    }

    /// <summary>
    /// Searches a use case that can handle the specified request and executes it asynchronously.
    /// If a validator exists for the request, it is also executed before the use case.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object for which to execute the use case.</typeparam>
    /// <param name="request">The request object for which to execute the use case.</param>
    /// <returns>The <see cref="Task"/> object representing the asynchronous execution.</returns>
    public Task ProcessAsync<TRequest>(TRequest request)
    {
        return ProcessAsync(request, CancellationToken.None);
    }

    /// <summary>
    /// Searches a use case that can handle the specified request and executes it asynchronously.
    /// If a validator exists for the request, it is also executed before the use case.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object for which to execute the use case.</typeparam>
    /// <param name="request">The request object for which to execute the use case.</param>
    /// <returns>The <see cref="Task"/> object representing the asynchronous execution.</returns>
    public async Task ProcessAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        ValidateRequest(request);
        object useCaseObject = InstantiateUseCaseFor<TRequest>();

        if (useCaseObject is IUseCase<TRequest> useCaseWithoutResponse)
        {
            await useCaseWithoutResponse.Execute(request, cancellationToken);
        }
        else
        {
            Type useCaseInterfaceType = useCaseObject.GetType().GetUseCaseInterface();

            if (useCaseInterfaceType.GetGenericTypeDefinition() == typeof(IUseCase<,>))
            {
                MethodInfo executeMethodInfo = useCaseInterfaceType.GetMethods()
                    .Single(x => IsExecuteMethod(x, typeof(TRequest)));

                await (Task)executeMethodInfo.Invoke(useCaseObject, new object[] { request, cancellationToken });
            }
            else
            {
                throw new UnusableUseCaseException(useCaseObject.GetType(), typeof(TRequest));
            }
        }
    }

    private void ValidateRequest<TRequest>(TRequest request)
    {
        Type requestType = typeof(TRequest);

        bool existsValidator = validators.ContainsKey(requestType);

        if (!existsValidator)
            return;

        Type validatorType = validators[requestType];
        object validatorObject = useCaseFactory.Create(validatorType);

        if (validatorObject is IRequestValidator<TRequest> validator)
            validator.Validate(request);
    }

    private object InstantiateUseCaseFor<TRequest>()
    {
        Type requestType = typeof(TRequest);

        if (!useCases.ContainsKey(requestType))
            throw new UseCaseNotFoundException();

        Type useCaseType = useCases[requestType];

        object useCaseObject = useCaseFactory.Create(useCaseType);

        if (useCaseObject == null)
            throw new UseCaseInstantiateException(useCaseType, requestType);

        return useCaseObject;
    }
}