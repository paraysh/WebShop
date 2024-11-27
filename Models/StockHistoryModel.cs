//Bearbeiter: Abbas Dayeh
//            Yusuf Can Sönmez
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert das Modell für die Lagerhistorie eines Artikels im WebShop.
    /// Diese Klasse enthält die grundlegenden Eigenschaften eines Artikels sowie eine Liste der Lagerbestände.
    /// </summary>
    public class StockHistoryModel
    {
        public StockHistoryModel()
        {
            lstStocks = new List<Stock>();
            lstStockDetails = new List<StockDetail>();
            lstInStockItems = new List<string>();
            
        }
        /// <summary>
        /// Die eindeutige Identifikationsnummer des Artikels.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Der Name des Artikels.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Der Typ des Artikels.
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// Die Kosten des Artikels.
        /// </summary>
        public decimal ItemCost { get; set; }

        /// <summary>
        /// Der Name des Bildes, das den Artikel darstellt.
        /// </summary>
        public string ItemImageName { get; set; }

        /// <summary>
        /// Eine Liste der Lagerbestände des Artikels.
        /// </summary>
        public List<Stock> lstStocks { get; set; }

        public List<OrderDetail> lstOrderDtls { get; set; }

        /// <summary>
        /// Eine Liste der Lagerdetails.
        /// </summary>
        public List<StockDetail> lstStockDetails { get; set; }

        public List<string> lstInStockItems { get; set; }
    }

    /// <summary>
    /// Repräsentiert einen Lagerbestand eines Artikels.
    /// Diese Klasse enthält die Eigenschaften eines Lagerbestands sowie eine Liste der Lagerdetails.
    /// </summary>
    public class Stock
    {
        public Stock()
        {
            lstSerialNumbers = new List<SerialNumbers>();
        }
        /// <summary>
        /// Die aktuelle Menge des Lagerbestands.
        /// </summary>
        public int StockCurrentQuantity { get; set; }

        /// <summary>
        /// Die anfängliche Menge des Lagerbestands.
        /// </summary>
        public int InitialQuantity { get; set; }

        public string MovementType { get; set; }
        public int TotalItemsAddedRemoved { get; set; }

        /// <summary>
        /// Der Benutzer, der den Lagerbestand hinzugefügt hat.
        /// </summary>
        public string StockAddedBy { get; set; }

        /// <summary>
        /// Das Datum, an dem der Lagerbestand hinzugefügt wurde.
        /// </summary>
        public string StockAddDate { get; set; }

        /// <summary>
        /// Der Benutzer, der den Lagerbestand gelöscht hat.
        /// </summary>
        public string StockDeletedBy { get; set; }

        /// <summary>
        /// Das Datum, an dem der Lagerbestand gelöscht wurde.
        /// </summary>
        public DateTime? StockDeleteDate { get; set; }

        public List<SerialNumbers> lstSerialNumbers { get; set; }
    }

    public class SerialNumbers
    {
        public string SerialNos { get; set; }
        public string DeleteReason { get; set; }
    }

    /// <summary>
    /// Repräsentiert die Details eines Lagerbestands.
    /// Diese Klasse enthält die Eigenschaften der Lagerdetails sowie Bestellinformationen.
    /// </summary>
    public class StockDetail
    {
        /// <summary>
        /// Die Seriennummer des Artikels.
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// Die eindeutige Identifikationsnummer der Bestellung.
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// Gibt an, ob der Artikel gelöscht wurde.
        /// </summary>
        public string IsDeleted { get; set; }

        public string DeleteReason { get; set; }

        /// <summary>
        /// Die eindeutige Identifikationsnummer der Bestellung.
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// Der Benutzer, der die Bestellung aufgegeben hat.
        /// </summary>
        public string OrderedBy { get; set; }

        /// <summary>
        /// Das Datum, an dem die Bestellung aufgegeben wurde.
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Gibt an, ob die Bestellung genehmigt wurde.
        /// </summary>
        public string OrderApproved { get; set; }

        /// <summary>
        /// Die Leihdauer in Monaten.
        /// </summary>
        public int LendingPeriodMonths { get; set; }

        /// <summary>
        /// Das Startdatum der Leihdauer.
        /// </summary>
        public DateTime? LendingStartDt { get; set; }

        /// <summary>
        /// Das Enddatum der Leihdauer.
        /// </summary>
        public DateTime? LendingEndDt { get; set; }
    }

    /// <summary>
    /// Repräsentiert die Details einer Bestellung.
    /// Diese Klasse enthält die Eigenschaften der Bestelldetails.
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// Die eindeutige Identifikationsnummer der Bestellung.
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// Der Benutzer, der die Bestellung aufgegeben hat.
        /// </summary>
        public string OrderedBy { get; set; }

        /// <summary>
        /// Das Datum, an dem die Bestellung aufgegeben wurde.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gibt an, ob die Bestellung genehmigt wurde.
        /// </summary>
        public string OrderApproved { get; set; }

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