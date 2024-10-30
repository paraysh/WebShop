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
    }
}