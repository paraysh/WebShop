using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class OrderModel : ItemModel
    {
        public OrderModel()
        {
            lstOrderDetails = new List<OrderDetail>();
        }
        //public int Id { get; set; }
        public string OrderId { get; set; }
        public int OrderedBy { get; set; }
        public DateTime OrderDt { get; set; }
        public string OrderApproved { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalCost { get; set; }
        public int TotalItems { get; set; }

        public decimal TeamBudget { get; set; }
        public decimal TeamLeaderBudget { get; set; }

        public int StockDetailsId { get; set; }
        public int LendingPeriodMonths { get; set; }
        public DateTime LendingStartDt { get; set; }
        public DateTime LendingEndDt { get; set; }
        public string OrderedByName { get; set; }
        public List<OrderDetail> lstOrderDetails { get; set; }
    }

    public class OrderDetail
    {
        public int? StockDetailsId { get; set; }
        public string SerialNo { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int LendingPeriodMonths { get; set; }
        public DateTime LendingStartDt { get; set; }
        public DateTime LendingEndDt { get; set; }
    }

}