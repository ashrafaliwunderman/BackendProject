using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.IValidation
{
    public interface IValidation
    {
        void AgainstNull(object? obj, HttpStatusCode code, string message);

    }
}
