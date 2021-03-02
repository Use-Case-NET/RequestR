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

        public static Type GetUseCaseInterface(this Type type)
        {
            if (!type.IsClass || type.IsAbstract)
                return null;

            return type.GetInterfaces().FirstOrDefault(IsUseCaseInterface);
        }

        private static bool IsUseCaseInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == typeof(IUseCase<,>) ||
                   genericTypeDefinition == typeof(IUseCase<>);
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
                   genericTypeDefinition == typeof(IRequestValidator<>);
        }
    }
}