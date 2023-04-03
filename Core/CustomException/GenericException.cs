using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.CustomException
{
    public class GenericException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public GenericException() : base()
        {

        }

        public GenericException(string message) : base(message)
        {

        }

        public GenericException(string message, params object[] args)
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public GenericException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
        public GenericException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
