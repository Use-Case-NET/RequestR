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
using System.Runtime.Serialization;

namespace DustInTheWind.RequestR
{
    /// <summary>
    /// The base exception thrown by the RequestR framework.
    /// </summary>
    [Serializable]
    public class RequestRException : Exception
    {
        private const string DefaultMessage = "An unknown error has occured in RequestR.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRException"/>
        /// with a default error message.
        /// </summary>
        public RequestRException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRException"/>
        /// with a custom error message.
        /// </summary>
        /// <param name="message">The message that described the encountered error.</param>
        public RequestRException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRException"/>
        /// with a custom error message an an inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that described the current exception.</param>
        /// <param name="inner">The exception that caused the current exception.</param>
        public RequestRException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRException"/>
        /// with serialized data.
        /// </summary>
        protected RequestRException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}