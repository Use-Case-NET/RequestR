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
/// Contains extension methods for the <see cref="RequestBus"/> type.
/// </summary>
public static class RequestBusExtensions
{
    /// <summary>
    /// Searches all the assemblies from the current <see cref="AppDomain"/>
    /// and registers all the use case and validator classes.
    /// </summary>
    /// <param name="requestBus">The <see cref="RequestBus"/> instance where to register the use cases and validators</param>
    public static void RegisterAllUseCases(this RequestBus requestBus)
    {
        if (requestBus == null) throw new ArgumentNullException(nameof(requestBus));

        AppDomain appDomain = AppDomain.CurrentDomain;

        Assembly[] assemblies = appDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
            RegisterAllInternal(requestBus, assembly);
    }

    /// <summary>
    /// Searches the specified assembly and registers all the use case and validator classes.
    /// </summary>
    /// <param name="requestBus">The <see cref="RequestBus"/> instance where to register the use cases and validators</param>
    /// <param name="assembly">The assembly to be searched.</param>
    public static void RegisterAllUseCases(this RequestBus requestBus, Assembly assembly)
    {
        if (requestBus == null) throw new ArgumentNullException(nameof(requestBus));
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));

        RegisterAllInternal(requestBus, assembly);
    }

    /// <summary>
    /// Searches the assembly containing the specified type and registers all the use case and validator classes.
    /// </summary>
    /// <param name="requestBus">The <see cref="RequestBus"/> instance where to register the use cases and validators</param>
    /// <param name="type">A <see cref="Type"/> instance contained by the assembly to be searched.</param>
    public static void RegisterAllUseCases(this RequestBus requestBus, Type type)
    {
        if (requestBus == null) throw new ArgumentNullException(nameof(requestBus));
        if (type == null) throw new ArgumentNullException(nameof(type));

        Assembly assembly = type.Assembly;

        RegisterAllInternal(requestBus, assembly);
    }

    private static void RegisterAllInternal(RequestBus requestBus, Assembly assembly)
    {
        IEnumerable<Type> handlerTypes = assembly.GetAllUseCases();

        foreach (Type handlerType in handlerTypes)
            requestBus.RegisterUseCase(handlerType);

        IEnumerable<Type> validatorTypes = assembly.GetAllRequestValidators();

        foreach (Type validatorType in validatorTypes)
            requestBus.RegisterValidator(validatorType);
    }
}