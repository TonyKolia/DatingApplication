//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatingApplication
{
    using System;
    using System.Collections.Generic;
    
    public partial class payments
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int payment_method { get; set; }
        public string means_of_payment_id { get; set; }
        public System.DateTime payment_date { get; set; }
        public decimal amount { get; set; }
    }
}
