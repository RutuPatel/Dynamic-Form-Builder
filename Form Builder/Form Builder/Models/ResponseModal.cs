using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Form_Builder.Models
{
    public class ResponseModal
    {
        public int ResponseId { get; set; }
        public Nullable<int> Id { get; set; }
        public string FormName { get; set; }
        public Nullable<int> FormId { get; set; }
        public string UserEmail { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string FieldResponse { get; set; }
     
    }
}