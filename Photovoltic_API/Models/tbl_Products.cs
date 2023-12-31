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
    
    public partial class tbl_Products
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> UserID { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<double> Wattage { get; set; }
        public Nullable<decimal> Efficiency { get; set; }
        public Nullable<int> WarrantyYears { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public string ImagePath { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<double> Powerpeak { get; set; }
        public string orientation { get; set; }
        public Nullable<double> inclination { get; set; }
        public Nullable<double> area { get; set; }
    }
}
