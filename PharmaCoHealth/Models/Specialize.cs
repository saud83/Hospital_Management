//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PharmaCoHealth.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Specialize
    {
        [Key]
        public int SpecializeId { get; set; }
        public string SpecializedField { get; set; }
        public Nullable<int> AdminID { get; set; }
    
        public virtual Administrator Administrator { get; set; }
    }
}
