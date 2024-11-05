using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class StockHistoryModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public decimal ItemCost { get; set; }
        public string ItemImageName { get; set; }

        public List<Stock> lstStocks { get; set; }
    }

    public class Stock
    {
        public int StockCurrentQuantity { get; set; }
        public int InitialQuantity { get; set; }
        public string StockAddedBy { get; set; }
        public DateTime StockAddDate { get; set; }
        public string StockDeletedBy { get; set; }
        public DateTime? StockDeleteDate { get; set; }
        public List<StockDetail> lstStockDetails { get; set; }
    }

    public class StockDetail
    {
        public string SerialNo { get; set; }
        public int? OrderId { get; set; }
        public string IsDeleted { get; set; }
        //public List<OrderDetails> lstOrderDetails { get; set; }

        public string OrderID { get; set; }
        public string OrderedBy { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderApproved { get; set; }
        public int LendingPeriodMonths { get; set; }
        public DateTime? LendingStartDt { get; set; }
        public DateTime? LendingEndDt { get; set; }
    }

    public class OrderDetails
    {
        public string OrderID { get; set; }
        public string OrderedBy { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderApproved { get; set; }
        public int LendingPeriodMonths { get; set; }
        public DateTime LendingStartDt { get; set; }
        public DateTime LendingEndDt { get; set; }
    }
}