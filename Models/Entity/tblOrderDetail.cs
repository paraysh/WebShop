//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebShop.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOrderDetail
    {
        public int Id { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> StockDetailsId { get; set; }
        public Nullable<int> LendingPeriod { get; set; }
        public Nullable<System.DateTime> LendingStartDt { get; set; }
        public Nullable<System.DateTime> LendingEndDt { get; set; }
    
        public virtual tblOrder tblOrder { get; set; }
        public virtual tblStockDetail tblStockDetail { get; set; }
    }
}