using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Form_Builder.Models
{
    public class ResponseFormModal
    {
        public int ResponseFormID { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
        public string UserEmail { get; set; }
        public string Name { get; set; }

        public virtual Form Form { get; set; }

        public ResponseModal ResponseModal { get; set; }
        //public List<ResponseForm> ResponseFormList { get; set; }
        public ResponseFormModal()
        {
            //ResponseFormList = new List<ResponseForm>();
        }
    }
   
}