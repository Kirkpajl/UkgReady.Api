using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Exceptions
{
    public sealed class UkgReadyServerException : UkgReadyException
    {
        public UkgReadyServerException() { }

        public UkgReadyServerException(string message) : base(message) { }

        public UkgReadyServerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public UkgReadyServerException(string message, Exception innerException) : base(message, innerException) { }

        public UkgReadyServerException(string message, string apiPath, HttpStatusCode statusCode, string content) : base(message, apiPath, statusCode, content) { }

        public UkgReadyServerException(string message, Exception innerException, string apiPath, HttpStatusCode statusCode, string content) : base(message, innerException, apiPath, statusCode, content) { }
    }
}
