using Microsoft.AspNet.Identity;
using Microsoft.AspNet;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Models.Enum;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WebShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        int _userRole;
        private WebShopEntities db;

        public OrderController()
        {
            db = new WebShopEntities();
        }
        public ActionResult Index()
        {
            List<OrderModel> model = new List<OrderModel>();

            var lstOrders = db.tblOrders.Include(x => x.tblOrderDetails).Include(x=> x.tblStockDetails).Include(x => x.tblUser).SelectMany(x => new List<OrderModel> { 
                new OrderModel
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    OrderedByName = x.tblUser.UserName,
                    OrderDt = x.OrderDate.Value,
                    OrderApproved = x.OrderApproved,
                    TotalItems = x.TotalItems.Value,
                    TotalCost = x.TotalCost.Value,
                    lstOrderDetails = x.tblOrderDetails.SelectMany(s => new List<OrderDetail> { new OrderDetail { 
                        StockDetailsId = s.StockDetailsId.Value,
                        SerialNo = s.tblStockDetail.SerialNumber,
                        ItemName = s.tblStockDetail.tblStock.tblItem.Name,
                        LendingPeriodMonths = s.LendingPeriodMonths.Value,
                        LendingStartDt = s.LendingStartDt.Value,
                        LendingEndDt = s.LendingEndDt.Value
                    } }).ToList()
                }
            }).ToList();

            //var lstStockDetails = lstOrders.SelectMany(x => x.tblStockDetails).ToList();

            return View(lstOrders);
        }

        public ActionResult Approve()
        {
            return Json(data: new { Success = true, Message = "Order Approved" }, JsonRequestBehavior.AllowGet);
        }
    }
}