using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models.Entity;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert ein Artikelmodell im WebShop.
    /// Diese Klasse enthält die grundlegenden Eigenschaften eines Artikels, wie Name, Beschreibung, Typ, Kosten und Bilddaten.
    /// </summary>
    public class ItemModel
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="ItemModel"/> Klasse.
        /// </summary>
        public ItemModel()
        {
            //this.tblStocks = new HashSet<tblStock>();
        }

        /// <summary>
        /// Die eindeutige Identifikationsnummer des Artikels.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Der Name des Artikels.
        /// </summary>
        [Required(ErrorMessage = "Name ist erforderlich")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Die Beschreibung des Artikels.
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// Der Typ des Artikels.
        /// </summary>
        [Required]
        [Display(Name = "Artikelart")]
        public int Type { get; set; }

        /// <summary>
        /// Die Kosten des Artikels in Euro pro Monat.
        /// </summary>
        [Required(ErrorMessage = "Kosten sind erforderlich")]
        [Display(Name = "Kosten (€ pro Monat)")]
        public Nullable<decimal> Cost { get; set; }

        /// <summary>
        /// Der Name des Bildes, das den Artikel darstellt.
        /// </summary>
        [Display(Name = "Bild")]
        public string ImageName { get; set; }

        /// <summary>
        /// Die Bilddaten des Artikels.
        /// </summary>
        public HttpPostedFileBase ImageData { get; set; }

        /// <summary>
        /// Gibt an, ob der Artikel aktiv ist.
        /// </summary>
        public string IsActive { get; set; } = "Y";

        //public virtual tblItemTypeMaster tblItemTypeMaster { get; set; }
        //public virtual ICollection<tblStock> tblStocks { get; set; }
    }
}