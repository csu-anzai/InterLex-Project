using System;
using System.Collections.Generic;
using System.Text;

namespace Interlex.Models.RequestModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserResetModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(Constants.PasswordRegex, ErrorMessage = Constants.PasswordErrorMessage)]
        [Compare(nameof(ConfirmPassword))]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}
