using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class FormFieldModel
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string IsRequired { get; set; }
        public int SortIndex { get; set; }
        public string Icons { get; set; }
        public string Class { get; set; }

        [Required]
        public string FieldResponse { get; set; }

        public string StreetAdress { get; set; }
        public string Adressline2 { get; set; }
        public string City  { get; set; }
        public string PinCode { get; set; }
        public string country { get; set; }

        public string state { get; set; }
        public string Hobby { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public int FormDetailId { get; set; }

        public int FormId { get; set; }
        public string FormName { get; set; }
        public string FormDescription { get; set; }

    
        public bool IsRunMode { get; set; }


        public List<FormFieldModel> FormFieldModelList { get; set; }

        public FormFieldModel()
        {
            FormFieldModelList = new List<FormFieldModel>();
            country = "";
            state = "";
        }
    }
}