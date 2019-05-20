using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.ResponseModels
{
    public class UserLoggedModel
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public UserPrivileges Privileges { get; set; }

        public string Username { get; set; }
    }


    public enum UserPrivileges
    {
        User = 0,
        Admin,
        SuperAdmin
    }

}
