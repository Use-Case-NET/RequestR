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

using System.Threading.Tasks;

namespace DustInTheWind.RequestR
{
    /// <summary>
    /// Implements a use case that can handle asynchronously a <typeparamref name="TRequest"/> object
    /// and returns a <typeparamref name="TResponse"/> object as response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request that can be handled by this use case.</typeparam>
    /// <typeparam name="TResponse">The type of the response that is returned by this use case.</typeparam>
    public interface IUseCase<in TRequest, TResponse>
    {
        /// <summary>
        /// Asynchronously executes the use case for the specified input data and returns the response.
        /// </summary>
        /// <param name="request">The object containing the input data for the use case.</param>
        /// <returns>The <see cref="Task"/> object that represents the asynchronous use case execution. The <see cref="Task{T}.Result"/> contains the use case's response.</returns>
        Task<TResponse> Execute(TRequest request);
    }
}
