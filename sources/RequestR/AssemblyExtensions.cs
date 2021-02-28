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
using System.Reflection;

namespace DustInTheWind.RequestR
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetAllRequestHandlersOrValidators(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Type[] handlerOrValidatorTypes = assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetInterfaces().Any(IsRequestHandlerOrValidatorInterface))
                .ToArray();

            foreach (Type handlerOrValidatorType in handlerOrValidatorTypes)
                yield return handlerOrValidatorType;
        }

        private static bool IsRequestHandlerOrValidatorInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == typeof(IRequestHandler<,>) ||
                   genericTypeDefinition == typeof(IRequestHandler<>) ||
                   genericTypeDefinition == typeof(IRequestValidator<>);
        }

        public static IEnumerable<Type> GetAllRequestHandlers(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Type[] handlerOrValidatorTypes = assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetInterfaces().Any(IsRequestHandlerInterface))
                .ToArray();

            foreach (Type handlerOrValidatorType in handlerOrValidatorTypes)
                yield return handlerOrValidatorType;
        }

        private static bool IsRequestHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();

            return genericTypeDefinition == typeof(IRequestHandler<,>) ||
                   genericTypeDefinition == typeof(IRequestHandler<>);
        }

        public static IEnumerable<Type> GetAllRequestValidators(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Type[] handlerOrValidatorTypes = assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract)
                .Where(x => x.GetInterfaces().Any(IsRequestValidatorInterface))
                .ToArray();

            foreach (Type handlerOrValidatorType in handlerOrValidatorTypes)
                yield return handlerOrValidatorType;
        }

        private static bool IsRequestValidatorInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequestValidator<>);
        }
    }
}