using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models.Enum
{
    public enum ItemTypeEnum
    {
        [Display(Name = "Hardware")]
        Hardware = 10,
        [Display(Name = "Rental Software")]
        RentalSoftware = 20,
        [Display(Name = "Licensed Software")]
        LicensedSoftware = 30
    }
}