using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Exceptions
{
    public class PasswordChangeException : Exception
    {
        public PasswordChangeException(string message) : base(message)
        {
        }

        public PasswordChangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PasswordChangeException()
        {
        }
    }
}
