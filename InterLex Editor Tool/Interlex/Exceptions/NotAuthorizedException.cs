using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
        {

        }

        public NotAuthorizedException(string message) : base(message)
        {
        }

        public NotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
