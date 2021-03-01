// RequestR
// Copyright (C) 2021 Dust in the Wind
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DustInTheWind.RequestR
{
    public class RequestBus
    {
        private readonly UseCaseFactoryBase useCaseFactory;
        private readonly Dictionary<Type, Type> useCases = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Type> validators = new Dictionary<Type, Type>();

        public RequestBus()
        {
            useCaseFactory = new DefaultUseCaseFactory();
        }

        public RequestBus(UseCaseFactoryBase useCaseFactory)
        {
            this.useCaseFactory = useCaseFactory ?? throw new ArgumentNullException(nameof(useCaseFactory));
        }

        public void RegisterUseCase<THandler>()
        {
            RegisterUseCase(typeof(THandler));
        }

        public void RegisterUseCase(Type useCaseType)
        {
            Type interfaceType = useCaseType.GetUseCaseInterface();

            if (interfaceType == null)
            {
                string message = "The specified type is not a use case. It must implement one of the " + typeof(IUseCase<,>).FullName + " or " + typeof(IUseCase<>).FullName + " interfaces.";
                throw new ArgumentException(message, nameof(useCaseType));
            }

            Type requestType = interfaceType.GenericTypeArguments[0];

            if (useCases.ContainsKey(requestType))
                throw new UseCaseAlreadyRegisteredException(requestType);

            useCases.Add(requestType, useCaseType);
        }

        public void RegisterValidator<TValidator>()
        {
            RegisterValidator(typeof(TValidator));
        }

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

        public TResponse Send<TRequest, TResponse>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!useCases.ContainsKey(requestType))
                throw new UseCaseNotFoundException();

            ValidateRequest(request);

            Type useCaseType = useCases[requestType];
            object useCaseObject = useCaseFactory.Create(useCaseType);

            switch (useCaseObject)
            {
                case IUseCase<TRequest, TResponse> useCaseWithResponse:
                    return useCaseWithResponse.Execute(request).Result;

                case IUseCase<TRequest> useCaseWithoutResponse:
                    useCaseWithoutResponse.Execute(request).Wait();
                    return default;

                default:
                    throw new UnusableUseCaseException(requestType);
            }
        }

        public void Send<TRequest>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!useCases.ContainsKey(requestType))
                throw new UseCaseNotFoundException();

            ValidateRequest(request);

            Type useCaseType = useCases[requestType];
            object useCaseObject = useCaseFactory.Create(useCaseType);

            switch (useCaseObject)
            {
                case IUseCase<TRequest> useCaseWithoutResponse:
                    useCaseWithoutResponse.Execute(request).Wait();
                    break;

                case IUseCase<TRequest, dynamic> useCaseWithResponse:
                    useCaseWithResponse.Execute(request).Wait();
                    break;

                default:
                    throw new UnusableUseCaseException(requestType);
            }
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!useCases.ContainsKey(requestType))
                throw new UseCaseNotFoundException();

            ValidateRequest(request);

            Type useCaseType = useCases[requestType];
            object useCaseObject = useCaseFactory.Create(useCaseType);

            switch (useCaseObject)
            {
                case IUseCase<TRequest, TResponse> useCaseWithResponse:
                    return await useCaseWithResponse.Execute(request);

                case IUseCase<TRequest> useCaseWithoutResponse:
                    await useCaseWithoutResponse.Execute(request);
                    return default;

                default:
                    throw new UnusableUseCaseException(requestType);
            }
        }

        public async Task SendAsync<TRequest>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!useCases.ContainsKey(requestType))
                throw new UseCaseNotFoundException();

            ValidateRequest(request);

            Type useCaseType = useCases[requestType];
            object useCaseObject = useCaseFactory.Create(useCaseType);

            switch (useCaseObject)
            {
                case IUseCase<TRequest> useCaseWithoutResponse:
                    await useCaseWithoutResponse.Execute(request);
                    break;

                case IUseCase<TRequest, dynamic> useCaseWithResponse:
                    await useCaseWithResponse.Execute(request);
                    break;

                default:
                    throw new UnusableUseCaseException(requestType);
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
    }
}