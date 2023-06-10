using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UkgReady.Api.Exceptions
{
    public abstract class UkgReadyException : Exception
    {
        protected UkgReadyException() { }

        protected UkgReadyException(string message) : base(message) { }

        protected UkgReadyException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected UkgReadyException(string message, Exception innerException) : base(message, innerException) { }

        protected UkgReadyException(string message, string apiPath, HttpStatusCode statusCode, string content) : base(message)
        {
            ApiPath = apiPath;
            StatusCode = statusCode;
            Content = content;
        }

        protected UkgReadyException(string message, Exception innerException, string apiPath, HttpStatusCode statusCode, string content) : base(message, innerException)
        {
            ApiPath = apiPath;
            StatusCode = statusCode;
            Content = content;
        }



        public string ApiPath { get; }
        public HttpStatusCode StatusCode { get; }
        public string Content { get; }
    }
}
