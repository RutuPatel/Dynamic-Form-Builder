//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Form_Builder
{
    using System;
    using System.Collections.Generic;
    
    public partial class Response
    {
        public int ResponseId { get; set; }
        public Nullable<int> Id { get; set; }
        public string FormName { get; set; }
        public Nullable<int> FormId { get; set; }
        public string UserEmail { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string FieldResponse { get; set; }
    
        public virtual Form Form { get; set; }
        public virtual FormField FormField { get; set; }
    }
}