//Bearbeiter: Hamza Elouari
//            Bekir Kurtuldu
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    /// <summary>
    /// Das OrderModel repräsentiert eine Bestellung im WebShop.
    /// Es erbt von ItemModel und enthält zusätzliche Informationen zur Bestellung wie Bestelldatum, Bestellstatus, Gesamtkosten und Details zu den bestellten Artikeln.
    /// </summary>
    public class OrderModel : ItemModel
    {
        /// <summary>
        /// Konstruktor, der die Liste der Bestelldetails initialisiert.
        /// </summary>
        public OrderModel()
        {
            lstOrderDetails = new List<OrderDetail>();
        }

        /// <summary>
        /// Die eindeutige ID der Bestellung.
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
        public string OrderApproved { get; set; }

        /// <summary>
        /// Der Status der Bestellung.
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Die Gesamtkosten der Bestellung.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Die Gesamtanzahl der Artikel in der Bestellung.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Das Budget des Teams.
        /// </summary>
        public decimal TeamBudget { get; set; }

        /// <summary>
        /// Das Budget des Teamleiters.
        /// </summary>
        public decimal TeamLeaderBudget { get; set; }

        /// <summary>
        /// Die ID der Bestandsdetails.
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

        /// <summary>
        /// Der Name des Benutzers, der die Bestellung aufgegeben hat.
        /// </summary>
        public string OrderedByName { get; set; }

        /// <summary>
        /// Die Liste der Bestelldetails.
        /// </summary>
        public List<OrderDetail> lstOrderDetails { get; set; }
    }

    /// <summary>
    /// Die OrderDetail-Klasse repräsentiert die Details eines einzelnen Artikels in einer Bestellung.
    /// </summary>
    public class OrderDetail
    {
        /// <summary>
        /// Die ID der Bestandsdetails (optional).
        /// </summary>
        public int? StockDetailsId { get; set; }

        /// <summary>
        /// Die Seriennummer des Artikels.
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// Die ID des Artikels.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Der Name des Artikels.
        /// </summary>
        public string ItemName { get; set; }

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

    /// <summary>
    /// Das WebGridFilterModel wird verwendet, um die Filterung und Sortierung von Bestellungen in einer WebGrid-Ansicht zu unterstützen.
    /// </summary>
    public class WebGridFilterModel
    {
        /// <summary>
        /// Die Liste der Bestellungen, nach denen gefiltert oder sortiert wird.
        /// </summary>
        public IEnumerable<OrderModel> OrderBy { get; set; }

        /// <summary>
        /// Der Überschriftstext für die Filter- oder Sortierspalte.
        /// </summary>
        public string HeadingText { get; set; }

        /// <summary>
        /// Die Eigenschaft, nach der gefiltert oder sortiert wird.
        /// </summary>
        public Func<OrderModel, string> Property { get; set; }
    }
}