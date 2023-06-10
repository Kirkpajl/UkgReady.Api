using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Exceptions
{
    public sealed class UkgReadyOperationException : UkgReadyException
    {
        public UkgReadyOperationException() { }

        public UkgReadyOperationException(string message) : base(message) { }

        public UkgReadyOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public UkgReadyOperationException(string message, Exception innerException) : base(message, innerException) { }

        public UkgReadyOperationException(string message, string apiPath, HttpStatusCode statusCode, string content) : base(message, apiPath, statusCode, content) { }

        public UkgReadyOperationException(string message, Exception innerException, string apiPath, HttpStatusCode statusCode, string content) : base(message, innerException, apiPath, statusCode, content) { }
    }
}
