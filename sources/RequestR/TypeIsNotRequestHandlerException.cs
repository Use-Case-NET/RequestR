using System;
using System.Collections.Generic;
using System.Linq;

namespace DustInTheWind.RequestR
{
    [Serializable]
    public class TypeIsNotRequestHandlerException : Exception
    {
        private const string DefaultMessageTemplate = "The type {0} is not a request handler. It must implement one of the interfaces: {1}.";

        public TypeIsNotRequestHandlerException(Type type)
            : base(BuildMessage(type))
        {
        }

        private static string BuildMessage(Type type)
        {
            Type[] requestHandlerInterfaces = {
                typeof(IRequestHandler<,>),
                typeof(IRequestHandler<>),
                typeof(IRequestHandlerAsync<,>),
                typeof(IRequestHandlerAsync<>),
            };

            IEnumerable<string> requestHandlerInterfacesNames = requestHandlerInterfaces
                .Select(x => x.FullName);

            return string.Format(DefaultMessageTemplate, type.FullName, string.Join<string>(", ", requestHandlerInterfacesNames));
        }
    }
}