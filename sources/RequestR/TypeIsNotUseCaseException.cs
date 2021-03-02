using System;
using System.Collections.Generic;
using System.Linq;

namespace DustInTheWind.RequestR
{
    [Serializable]
    public class TypeIsNotUseCaseException : Exception
    {
        private const string DefaultMessageTemplate = "The type {0} is not a use case. It must implement one of the interfaces: {1}.";

        public TypeIsNotUseCaseException(Type type)
            : base(BuildMessage(type))
        {
        }

        private static string BuildMessage(Type type)
        {
            Type[] useCaseInterfaces = {
                typeof(IUseCase<,>),
                typeof(IUseCase<>)
            };

            IEnumerable<string> useCaseInterfacesNames = useCaseInterfaces
                .Select(x => x.FullName);

            return string.Format(DefaultMessageTemplate, type.FullName, string.Join<string>(", ", useCaseInterfacesNames));
        }
    }
}