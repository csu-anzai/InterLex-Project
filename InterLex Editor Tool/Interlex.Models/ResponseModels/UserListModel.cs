using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.ResponseModels
{
    public class UserListModel
    {
        public string Username { get; set; }

        public bool IsActive { get; set; }

        public string Organization { get; set; }

        public string Privileges { get; set; }
    }
}
