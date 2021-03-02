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

namespace DustInTheWind.RequestR
{
    /// <summary>
    /// Exception thrown when the application needs a <see cref="Type"/> object that represents a use case,
    /// but it receives one that does not implement the use case interfaces.
    /// </summary>
    [Serializable]
    public class TypeIsNotUseCaseException : Exception
    {
        private const string DefaultMessageTemplate = "The type {0} is not a use case. It must implement one of the interfaces: {1}.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeIsNotUseCaseException"/> class
        /// with the <see cref="Type"/> instance that is expected to represent a use case, but it doesn't.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance that is expected to represent a use case, but it doesn't.</param>
        public TypeIsNotUseCaseException(Type type)
            : base(BuildMessage(type))
        {
        }

        private static string BuildMessage(Type type)
        {
            Type[] useCaseInterfaces = {
                typeof(IUseCase<,>),
                typeof(IUseCase<>)
            };

            IEnumerable<string> useCaseInterfacesNames = useCaseInterfaces
                .Select(x => x.FullName);

            return string.Format(DefaultMessageTemplate, type.FullName, string.Join<string>(", ", useCaseInterfacesNames));
        }
    }
}