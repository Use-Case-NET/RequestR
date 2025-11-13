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
/// Exception thrown when the use case found in the registered use cases list for the specified request
/// cannot be used to handle the request.
/// </summary>
public class UnusableUseCaseException : RequestRException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnusableUseCaseException"/> class.
    /// </summary>
    public UnusableUseCaseException(Type useCaseType)
        : base($"The use case found cannot be used for the specified request. Use case type {useCaseType.FullName}.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnusableUseCaseException"/> class.
    /// </summary>
    /// <param name="useCaseType">The type of the use case that cannot be used.</param>
    /// <param name="requestType">The type of the request for which the use case cannot be used.</param>
    public UnusableUseCaseException(Type useCaseType, Type requestType)
        : base($"The use case found cannot be used for the specified request. Use case type {useCaseType.FullName}; Request type: {requestType.FullName}")
    {
    }
}
