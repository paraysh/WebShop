//Bearbeiter: Alper Daglioglu
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert ein Produktmodell im WebShop.
    /// Diese Klasse erbt von ItemModel und fügt zusätzliche Eigenschaften hinzu, die für die Verwaltung von Produkten erforderlich sind.
    /// </summary>
    public class ProductModel : ItemModel
    {
        /// <summary>
        /// Der Typ des Produkts.
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Die Anzahl der Artikel, die auf Lager sind.
        /// </summary>
        public int? ItemsInStock { get; set; }
    }
}