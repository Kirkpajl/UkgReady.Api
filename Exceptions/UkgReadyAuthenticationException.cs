using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Exceptions
{
    public class UkgReadyAuthenticationException : UkgReadyException
    {
        public UkgReadyAuthenticationException() { }

        public UkgReadyAuthenticationException(string message) : base(message) { }

        public UkgReadyAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public UkgReadyAuthenticationException(string message, Exception innerException) : base(message, innerException) { }

        public UkgReadyAuthenticationException(string message, string apiPath, HttpStatusCode statusCode, string content) : base(message, apiPath, statusCode, content) { }

        public UkgReadyAuthenticationException(string message, Exception innerException, string apiPath, HttpStatusCode statusCode, string content) : base(message, innerException, apiPath, statusCode, content) { }
    }
}
