using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class ProductModel : ItemModel
    {
        public string ProductType { get; set; }
        public int? ItemsInStock { get; set; }
    }
}