using Form_Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace WebApplication1.Models
{
    public class FormModel
    {
        public int? UserId { get; set; }
        
        public int FormId { get; set; }
        [Required]
        public string FormName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Please enter between 5-50 words")]
        public string FormDescription { get; set; }
        [Required]
        public string FormUrl { get; set; }
        [Required]
        public string HashUrl { get; set; }
        public int FieldCount { get; set; }
        public List<FormFieldModel> formField { get; set; }

        public List<User> userList { get; set; } 

        [Required]
        [EmailAddress]
        public string Sharetoemail { get; set; }
        
        public bool IsUrl { get; set; }

        public string country { get; set; }

        public string state { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public virtual ICollection<FormDetail> FormDetails { get; set; }



        //public List<FormModel> FormModelList { get; set; }
        public FormModel()
        {
            userList = new List<User>();
            //FormModelList = new List<FormModel>();
            formField = new List<FormFieldModel>();
        }

    }
}
