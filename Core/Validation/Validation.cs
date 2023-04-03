using Core.CustomException;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validation
{
    public class Validation : IValidation.IValidation
    {
        public void AgainstNull(object obj, HttpStatusCode code, string message)
        {
            if (obj == null)
            {
                throw new GenericException(code, message);
            }
        }
    }
}
