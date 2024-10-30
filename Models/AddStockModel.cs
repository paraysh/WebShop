﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class AddStockModel : ItemModel
    {
        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        public string ItemTypeStr { get; set; }
        public List<string> LstSerialNumbers { get; set; }
    }
}