using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Form_Builder.Models
{
    public class UserModel
    {

        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MembershipPassword(
                 MinRequiredNonAlphanumericCharacters = 1,
                 MinNonAlphanumericCharactersError = "Your password needs to contain at least one symbol (!, @, #, etc).",
                 ErrorMessage = "Your password must be 6 characters long and contain at least one symbol (!, @, #, etc).",
                 MinRequiredPasswordLength = 8
        )]
        public string Password { get; set; }
       
        public string ForgotPasswodID { get; set; }

        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string UserName { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.Guid> GUID { get; set; }

        public virtual ICollection<Form> Forms { get; set; }
    }
}

