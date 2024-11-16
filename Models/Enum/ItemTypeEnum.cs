using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models.Enum
{
    /// <summary>
    /// Repräsentiert die verschiedenen Typen von Artikeln im WebShop.
    /// Diese Enumeration wird verwendet, um die Art eines Artikels zu definieren.
    /// </summary>
    public enum ItemTypeEnum
    {
        /// <summary>
        /// Hardware-Artikel.
        /// </summary>
        [Display(Name = "Hardware")]
        Hardware = 10,

        /// <summary>
        /// Mietsoftware-Artikel.
        /// </summary>
        [Display(Name = "Mietsoftware")]
        RentalSoftware = 20,

        /// <summary>
        /// Lizenzsoftware-Artikel.
        /// </summary>
        [Display(Name = "Lizenzsoftware")]
        LicensedSoftware = 30
    }
}