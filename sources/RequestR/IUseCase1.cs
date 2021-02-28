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

namespace DustInTheWind.RequestR
{
    /// <summary>
    /// Implements a use case that can handle a <typeparamref name="TRequest"/> object
    /// and returns nothing.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request that can be handled by this use case.</typeparam>
    public interface IUseCase<in TRequest>
    {
        /// <summary>
        /// Executes the use case for the specified input data.
        /// </summary>
        /// <param name="request">The object containing the input data for the use case.</param>
        void Execute(TRequest request);
    }
}