using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using ResponseModels;

    public class UserRegisterModel
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Username { get; set; }


        [Required]
        [RegularExpression(Constants.PasswordRegex, ErrorMessage = Constants.PasswordErrorMessage)]
        [Compare(nameof(ConfirmPassword))]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        public OrganizationModel Organization { get; set; }

        [JsonRequired]
        [Enumeration]
        public UserPrivileges Privileges { get; set; }

    }
}
