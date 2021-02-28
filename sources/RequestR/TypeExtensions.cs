using System;
using System.Linq;

namespace DustInTheWind.RequestR
{
    internal static class TypeExtensions
    {
        public static bool IsUseCase(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return false;

            return type.GetInterfaces().Any(IsUseCaseInterface);
        }

        private static bool IsUseCaseInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == typeof(IUseCase<,>) ||
                   genericTypeDefinition == typeof(IUseCase<>) ||
                   genericTypeDefinition == typeof(IUseCaseAsync<,>) ||
                   genericTypeDefinition == typeof(IUseCaseAsync<>);
        }

        public static bool IsRequestValidator(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return false;

            return type.GetInterfaces().Any(IsRequestValidatorInterface);
        }

        private static bool IsRequestValidatorInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequestValidator<>);
        }

        public static bool IsUseCaseOrRequestValidator(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return false;

            return type.GetInterfaces().Any(IsUseCaseOrRequestValidatorInterface);
        }

        private static bool IsUseCaseOrRequestValidatorInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == typeof(IUseCase<,>) ||
                   genericTypeDefinition == typeof(IUseCase<>) ||
                   genericTypeDefinition == typeof(IUseCaseAsync<,>) ||
                   genericTypeDefinition == typeof(IUseCaseAsync<>) ||
                   genericTypeDefinition == typeof(IRequestValidator<>);
        }
    }
}