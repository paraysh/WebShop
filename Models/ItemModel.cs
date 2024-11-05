using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models.Entity;

namespace WebShop.Models
{
    public class ItemModel
    {
        public ItemModel()
        {
            //this.tblStocks = new HashSet<tblStock>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Item Type")]
        public int Type { get; set; }

        [Required]
        [Display(Name = "Cost")]
        public Nullable<decimal> Cost { get; set; }

        [Display(Name = "Image")]
        public string ImageName { get; set; }

        public HttpPostedFileBase ImageData { get; set; }
        public string IsActive { get; set; } = "Y";

        //public virtual tblItemTypeMaster tblItemTypeMaster { get; set; }
        //public virtual ICollection<tblStock> tblStocks { get; set; }
    }
}