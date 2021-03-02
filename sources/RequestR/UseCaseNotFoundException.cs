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

namespace DustInTheWind.RequestR
{
    /// <summary>
    /// This exception is thrown by <see cref="RequestBus"/> when no use case was registered
    /// for the specified request.
    /// </summary>
    public class UseCaseNotFoundException : RequestRException
    {
        private const string DefaultMessage = "No use case is registered for the specified request.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UseCaseNotFoundException"/> class.
        /// </summary>
        public UseCaseNotFoundException()
            : base(DefaultMessage)
        {
        }
    }
}