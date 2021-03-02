using System;
using System.Linq;

namespace DustInTheWind.RequestR
{
    internal static class TypeExtensions
    {
        public static bool IsRequestHandler(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return false;

            return type.GetInterfaces().Any(IsRequestHandlerInterface);
        }

        public static Type GetRequestHandlerInterface(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return null;

            return type.GetInterfaces().FirstOrDefault(IsRequestHandlerInterface);
        }

        private static bool IsRequestHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                   genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                   genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<,>) ||
                   genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<>);
        }

        public static bool IsRequestValidator(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return false;

            return type.GetInterfaces().Any(IsRequestValidatorInterface);
        }

        public static Type GetRequestValidatorInterface(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return null;

            return type.GetInterfaces().FirstOrDefault(IsRequestValidatorInterface);
        }

        private static bool IsRequestValidatorInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequestValidator<>);
        }

        public static bool IsRequestHandlerOrRequestValidator(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return false;

            return type.GetInterfaces().Any(IsRequestHandlerOrRequestValidatorInterface);
        }

        private static bool IsRequestHandlerOrRequestValidatorInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                   genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                   genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<,>) ||
                   genericTypeDefinition.GetGenericTypeDefinition() == typeof(IRequestHandlerAsync<>) ||
                   genericTypeDefinition == typeof(IRequestValidator<>);
        }
    }
}