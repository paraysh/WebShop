//Bearbeiter: Abbas Dayeh
//            Alper Daglioglu
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert das Modell für den Warenkorb im WebShop.
    /// Diese Klasse erbt von ItemModel und fügt zusätzliche Eigenschaften hinzu, die für die Verwaltung des Warenkorbs erforderlich sind.
    /// </summary>
    public class ShoppingCartModel : ItemModel
    {
        /// <summary>
        /// Die Menge der Artikel im Warenkorb.
        /// </summary>
        public int CartQuantity { get; set; }

        /// <summary>
        /// Der Stückpreis des Artikels.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Der Gesamtpreis der Artikel im Warenkorb.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Die eindeutige Identifikationsnummer der Bestellung.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Die ID des Benutzers, der die Bestellung aufgegeben hat.
        /// </summary>
        public int OrderedBy { get; set; }

        /// <summary>
        /// Das Datum, an dem die Bestellung aufgegeben wurde.
        /// </summary>
        public DateTime OrderDt { get; set; }

        /// <summary>
        /// Gibt an, ob die Bestellung genehmigt wurde.
        /// </summary>
        public char OrderApproved { get; set; }

        /// <summary>
        /// Die Gesamtkosten der Artikel im Warenkorb.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Die Gesamtanzahl der Artikel im Warenkorb.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Die eindeutige Identifikationsnummer der Lagerdetails.
        /// </summary>
        public int StockDetailsId { get; set; }

        /// <summary>
        /// Die Leihdauer in Monaten.
        /// </summary>
        public int LendingPeriodMonths { get; set; }

        /// <summary>
        /// Das Startdatum der Leihdauer.
        /// </summary>
        public DateTime LendingStartDt { get; set; }

        /// <summary>
        /// Das Enddatum der Leihdauer.
        /// </summary>
        public DateTime LendingEndDt { get; set; }
    }
}