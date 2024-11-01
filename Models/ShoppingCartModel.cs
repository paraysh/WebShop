using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class ShoppingCartModel : ItemModel
    {
        public int CartQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set;}
        public string OrderId { get; set; }
        public int OrderedBy { get; set; }
        public DateTime OrderDt { get; set; }
        public char OrderApproved { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalItems { get; set; }
        public int StockDetailsId { get; set; }
        public int LendingPeriodMonths { get; set; }
        public DateTime LendingStartDt { get; set; }
        public DateTime LendingEndDt { get; set; }
    }
}