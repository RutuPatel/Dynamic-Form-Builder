using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class EditFormModel
    {   
        public int FormDetailId { get; set; }
        public int? Id { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string Isrequired { get; set; }
        public int Sortindex { get; set; }
        public string Icons { get; set; }
        public string Class { get; set; }

        public int? UserId { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
        public string FormDescription { get; set; }
        public string FormUrl { get; set; }
        public int FieldCount { get; set; }

        

        public EditFormModel()
        {
            
        }

    }
}