using System;
using System.Runtime.Serialization;

namespace DustInTheWind.RequestR
{
    [Serializable]
    public class RequestRException : Exception
    {
        public RequestRException()
        {
        }

        public RequestRException(string message)
            : base(message)
        {
        }

        public RequestRException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RequestRException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}