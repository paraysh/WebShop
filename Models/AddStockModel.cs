using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebShop.Models
{
    /// <summary>
    /// Das AddStockModel repräsentiert das Modell zum Hinzufügen von Bestand im WebShop.
    /// Es erbt von ItemModel und enthält zusätzliche Informationen wie Menge, Seriennummern und Löschgrund.
    /// </summary>
    public class AddStockModel : ItemModel
    {
        /// <summary>
        /// Die Menge des hinzuzufügenden Bestands.
        /// </summary>
        [Required]
        [Display(Name = "Menge")]
        public int Quantity { get; set; }

        /// <summary>
        /// Der Typ des Artikels als Zeichenkette.
        /// </summary>
        public string ItemTypeStr { get; set; }

        /// <summary>
        /// Die Liste der Seriennummern des hinzuzufügenden Bestands.
        /// </summary>
        public List<string> LstSerialNumbers { get; set; }

        /// <summary>
        /// Die Liste der auswählbaren Seriennummern.
        /// </summary>
        public List<SelectListItem> SelectLstSerialNumbers { get; set; }

        /// <summary>
        /// Die ausgewählte Seriennummer.
        /// </summary>
        public string SelectedSerialNo { get; set; }

        /// <summary>
        /// Der Grund für das Löschen des Bestands.
        /// </summary>
        public string DeleteReason { get; set; }
    }
}