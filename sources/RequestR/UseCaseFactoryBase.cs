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

namespace DustInTheWind.RequestR;

/// <summary>
/// A factory class that creates instances of use cases.
/// </summary>
public abstract class UseCaseFactoryBase
{
    /// <summary>
    /// Creates a new instance of the specified use case type.
    /// The actual creation of the object is delegated to the inheritor class.
    /// See the <see cref="CreateInternal"/> method.
    /// </summary>
    /// <param name="type">The type of the use case to be created.</param>
    /// <returns>A new instance of the specified use case.</returns>
    public object CreateUseCase(Type type)
    {
        bool isUseCase = type.IsUseCase();

        if (!isUseCase)
            throw new NotUseCaseException(type);

        return CreateInternal(type);
    }

    public object CreateRequestValidator(Type type)
    {
        bool isRequestValidator = type.IsRequestValidator();
        
        if (!isRequestValidator)
            throw new NotRequestValidatorException(type);

        return CreateInternal(type);
    }

    /// <summary>
    /// When implemented by an inheritor, it creates a new instance of the specified use case.
    /// </summary>
    /// <param name="type">The type of the use case to be created.</param>
    /// <returns>A new instance of the specified use case.</returns>
    protected abstract object CreateInternal(Type type);
}
