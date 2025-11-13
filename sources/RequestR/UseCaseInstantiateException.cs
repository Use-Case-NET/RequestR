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

namespace DustInTheWind.RequestR;

/// <summary>
/// Exception thrown when the use case cannot be instantiated.
/// </summary>
public class UseCaseInstantiateException : RequestRException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseCaseInstantiateException"/> class
    /// with the use case type.
    /// </summary>
    /// <param name="useCaseType">The type of the use case class that could not be instantiated.</param>
    public UseCaseInstantiateException(Type useCaseType)
        : base($"The use case could not be created. Use case type: {useCaseType.FullName}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseCaseInstantiateException"/> class
    /// with the use case type.
    /// </summary>
    /// <param name="useCaseType">The type of the use case class that could not be instantiated.</param>
    /// <param name="requestType">The type of the request class for which the use case creation was attempted.</param>
    public UseCaseInstantiateException(Type useCaseType, Type requestType)
        : base($"The use case could not be created. Use case type: {useCaseType.FullName}; Request type: {requestType.FullName}")
    {
    }
}