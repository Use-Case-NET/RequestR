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
/// Contains extension methods for the <see cref="Assembly"/> type.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Enumerates all the types that implement the one of the interfaces: <see cref="IUseCase{TRequest}"/>,
    /// <see cref="IUseCase{TRequest,TResponse}"/> or <see cref="IRequestValidator{TRequest}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to be searched.</param>
    /// <returns>An enumeration of <see cref="Type"/> objects representing the use case and request validator classes.</returns>
    public static IEnumerable<Type> GetAllUseCasesAndRequestValidators(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));

        Type[] handlerOrValidatorTypes = assembly.GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract)
            .Where(x => x.GetInterfaces().Any(IsUseCaseOrValidatorInterface))
            .ToArray();

        foreach (Type handlerOrValidatorType in handlerOrValidatorTypes)
            yield return handlerOrValidatorType;
    }

    private static bool IsUseCaseOrValidatorInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        Type genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IUseCase<,>) ||
               genericTypeDefinition == typeof(IUseCase<>) ||
               genericTypeDefinition == typeof(IRequestValidator<>);
    }

    /// <summary>
    /// Enumerates all the types that implement the one of the interfaces: <see cref="IUseCase{TRequest}"/>,
    /// <see cref="IUseCase{TRequest}"/>, <see cref="IUseCase{TRequest}"/> or
    /// <see cref="IUseCase{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="assembly">The assembly to be searched.</param>
    /// <returns>An enumeration of <see cref="Type"/> objects representing the use case classes.</returns>
    public static IEnumerable<Type> GetAllUseCases(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));

        Type[] handlerOrValidatorTypes = assembly.GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract)
            .Where(x => x.GetInterfaces().Any(IsUseCaseInterface))
            .ToArray();

        foreach (Type handlerOrValidatorType in handlerOrValidatorTypes)
            yield return handlerOrValidatorType;
    }

    private static bool IsUseCaseInterface(Type type)
    {
        if (!type.IsGenericType)
            return false;

        Type genericTypeDefinition = type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(IUseCase<,>) ||
               genericTypeDefinition == typeof(IUseCase<>);
    }

    /// <summary>
    /// Enumerates all the types that implement the <see cref="IRequestValidator{TRequest}"/> interface.
    /// </summary>
    /// <param name="assembly">The assembly to be searched.</param>
    /// <returns>An enumeration of <see cref="Type"/> objects representing the request validator classes.</returns>
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