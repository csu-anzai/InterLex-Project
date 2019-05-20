using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models
{
    internal static class Constants
    {
        public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[\d])(?=.*[A-Z])\S{6,}$";

        public const string PasswordErrorMessage =
            "Password must be at least 6 characters long, must contain at least one lowercase, one uppercase and one digit character.";
    }
}
