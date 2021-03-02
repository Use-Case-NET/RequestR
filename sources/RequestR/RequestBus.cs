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
using System.Linq;
using System.Threading.Tasks;

namespace DustInTheWind.RequestR
{
    public class RequestBus
    {
        private readonly IRequestHandlerFactory requestHandlerFactory;
        private readonly Dictionary<Type, Type> handlers = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Type> validators = new Dictionary<Type, Type>();

        public RequestBus()
        {
            requestHandlerFactory = new DefaultRequestHandlerFactory();
        }

        public RequestBus(IRequestHandlerFactory requestHandlerFactory)
        {
            this.requestHandlerFactory = requestHandlerFactory ?? throw new ArgumentNullException(nameof(requestHandlerFactory));
        }

        public void RegisterHandler<THandler>()
        {
            RegisterHandler(typeof(THandler));
        }

        public void RegisterHandler(Type requestHandlerType)
        {
            Type interfaceType = requestHandlerType.GetRequestHandlerInterface(); 
            
            if (interfaceType == null)
                throw new TypeIsNotRequestHandlerException(requestHandlerType);

            Type requestType = interfaceType.GenericTypeArguments[0];

            if (handlers.ContainsKey(requestType))
                throw new HandlerAlreadyRegisteredException(requestType);

            handlers.Add(requestType, requestHandlerType);
        }

        public void RegisterValidator<TValidator>()
        {
            RegisterValidator(typeof(TValidator));
        }

        public void RegisterValidator(Type validatorType)
        {
            Type interfaceType = validatorType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequestValidator<>));

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

            if (!handlers.ContainsKey(requestType))
                throw new HandlerNotFoundException();

            ValidateRequest(request);

            Type requestHandlerType = handlers[requestType];
            object requestHandlerObject = requestHandlerFactory.Create(requestHandlerType);

            switch (requestHandlerObject)
            {
                case IRequestHandler<TRequest, TResponse> requestHandlerWithResponse:
                    return requestHandlerWithResponse.Handle(request);

                case IRequestHandler<TRequest> requestHandlerWithoutResponse:
                    requestHandlerWithoutResponse.Handle(request);
                    return default;

                case IRequestHandlerAsync<TRequest, TResponse> requestHandlerWithResponse:
                    return requestHandlerWithResponse.Handle(request).Result;

                case IRequestHandlerAsync<TRequest> requestHandlerWithoutResponse:
                    requestHandlerWithoutResponse.Handle(request).Wait();
                    return default;

                default:
                    throw new UnusableRequestHandlerException(requestType);
            }
        }

        public void Send<TRequest>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!handlers.ContainsKey(requestType))
                throw new HandlerNotFoundException();

            ValidateRequest(request);

            Type requestHandlerType = handlers[requestType];
            object requestHandlerObject = requestHandlerFactory.Create(requestHandlerType);

            switch (requestHandlerObject)
            {
                case IRequestHandler<TRequest> requestHandlerWithResponse:
                    requestHandlerWithResponse.Handle(request);
                    break;

                case IRequestHandler<TRequest, object> requestHandlerWithoutResponse:
                    requestHandlerWithoutResponse.Handle(request);
                    break;

                case IRequestHandlerAsync<TRequest> requestHandlerWithResponse:
                    requestHandlerWithResponse.Handle(request).Wait();
                    break;

                case IRequestHandlerAsync<TRequest, object> requestHandlerWithoutResponse:
                    requestHandlerWithoutResponse.Handle(request).Wait();
                    break;

                default:
                    throw new UnusableRequestHandlerException(requestType);
            }
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!handlers.ContainsKey(requestType))
                throw new HandlerNotFoundException();

            ValidateRequest(request);

            Type requestHandlerType = handlers[requestType];
            object requestHandler = requestHandlerFactory.Create(requestHandlerType);

            switch (requestHandler)
            {
                case IRequestHandler<TRequest, TResponse> requestHandlerWithResponse:
                    return requestHandlerWithResponse.Handle(request);

                case IRequestHandler<TRequest> requestHandlerWithoutResponse:
                    requestHandlerWithoutResponse.Handle(request);
                    return default;
                    
                case IRequestHandlerAsync<TRequest, TResponse> requestHandlerWithResponse:
                    return await requestHandlerWithResponse.Handle(request);

                case IRequestHandlerAsync<TRequest> requestHandlerWithoutResponse:
                    await requestHandlerWithoutResponse.Handle(request);
                    return default;

                default:
                    throw new UnusableRequestHandlerException(requestType);
            }
        }

        public async Task SendAsync<TRequest>(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Type requestType = typeof(TRequest);

            if (!handlers.ContainsKey(requestType))
                throw new HandlerNotFoundException();

            ValidateRequest(request);

            Type requestHandlerType = handlers[requestType];
            object requestHandler = requestHandlerFactory.Create(requestHandlerType);

            switch (requestHandler)
            {
                case IRequestHandler<TRequest> requestHandlerWithoutResponse:
                    requestHandlerWithoutResponse.Handle(request);
                    break;

                case IRequestHandler<TRequest, object> requestHandlerWithResponse:
                    requestHandlerWithResponse.Handle(request);
                    break;

                case IRequestHandlerAsync<TRequest> requestHandlerWithoutResponse:
                    await requestHandlerWithoutResponse.Handle(request);
                    break;

                case IRequestHandlerAsync<TRequest, object> requestHandlerWithResponse:
                    await requestHandlerWithResponse.Handle(request);
                    break;

                default:
                    throw new UnusableRequestHandlerException(requestType);
            }
        }

        private void ValidateRequest<TRequest>(TRequest request)
        {
            Type requestType = typeof(TRequest);

            bool existsValidator = validators.ContainsKey(requestType);

            if (!existsValidator)
                return;

            Type validatorType = validators[requestType];
            object validatorObject = requestHandlerFactory.Create(validatorType);

            if (validatorObject is IRequestValidator<TRequest> validator)
                validator.Validate(request);
        }
    }
}