using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Exceptions
{
    public class UkgReadyAuthorizationException : UkgReadyException
    {
        public UkgReadyAuthorizationException() { }

        public UkgReadyAuthorizationException(string message) : base(message) { }

        public UkgReadyAuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public UkgReadyAuthorizationException(string message, Exception innerException) : base(message, innerException) { }

        public UkgReadyAuthorizationException(string message, string apiPath, HttpStatusCode statusCode, string content) : base(message, apiPath, statusCode, content) { }

        public UkgReadyAuthorizationException(string message, Exception innerException, string apiPath, HttpStatusCode statusCode, string content) : base(message, innerException, apiPath, statusCode, content) { }
    }
}
