//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Photovoltic_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_ProductAssignment
    {
        public int ID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> UserID { get; set; }
        public string UserName { get; set; }
        public Nullable<double> Powerpeak { get; set; }
        public string orientation { get; set; }
        public Nullable<double> inclination { get; set; }
        public Nullable<double> area { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string ImagePath { get; set; }
        public string LatitudeNew { get; set; }
        public string LongitudeNew { get; set; }
    }
}
