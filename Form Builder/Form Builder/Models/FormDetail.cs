using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Form_Builder.Models
{
    public class FormDetail
    {

        public int FormDetailId { get; set; }
        [ForeignKey("Form")]
        public int FormId { get; set; }
        [ForeignKey("FormField")]
        public int Id { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string Isrequired { get; set; }
        public int Sortindex { get; set; }
        public string Icons { get; set; }
        public string Class { get; set; }

        public List<FormDetail> FormDetailList { get; set; }

        public FormDetail()
        {
            FormDetailList = new List<FormDetail>();
        }

    }
}




