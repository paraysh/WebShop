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
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            List<OrderModel> model = new List<OrderModel>();

            var lstOrders = db.tblOrders.Include(x => x.tblOrderDetails).Include(x => x.tblStockDetails).Include(x => x.tblUser).SelectMany(x => new List<OrderModel> {
                new OrderModel
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    OrderedByName = x.tblUser.UserName,
                    OrderDt = x.OrderDate.Value,
                    OrderApproved = x.OrderApproved,
                    OrderStatus = x.OrderApproved == "R" ? "Rejected" : (x.OrderApproved == "Y" ? "Approved" : "Pending Approval"),
                    TotalItems = x.TotalItems.Value,
                    TotalCost = x.TotalCost.Value,
                    lstOrderDetails = x.tblOrderDetails.SelectMany(s => new List<OrderDetail> { new OrderDetail {
                        StockDetailsId = s.tblStockDetail == null ? (int?)null : s.StockDetailsId.Value,
                        SerialNo = s.tblStockDetail == null ?  "NA": s.tblStockDetail.SerialNumber,
                        ItemName = s.tblStockDetail == null ? s.tblItem.Name : s.tblStockDetail.tblStock.tblItem.Name,
                        LendingPeriodMonths = s.LendingPeriodMonths.Value,
                        LendingStartDt = s.LendingStartDt.Value,
                        LendingEndDt = s.LendingEndDt.Value
                    } }).ToList()
                }
            }).ToList();

            //var lstStockDetails = lstOrders.SelectMany(x => x.tblStockDetails).ToList();

            return View(lstOrders);
        }

        public ActionResult Approve(int OrderId)
        {
            var OrderTblRow = db.tblOrders.Where(x => x.Id == OrderId).Single();

            var currUserName = "Employee"; //User.Identity.GetUserName();
            var currUserObj = db.tblUsers.Where(x => x.UserName == currUserName).Single();
            var employeeBudget = currUserObj.tblTeamEmployees.Where(x => x.TeamEmployeeId == currUserObj.Id).Single().TeamEmployeeBudget;
            var utilisedBudget = db.tblOrders.Where(x => x.OrderedBy == currUserObj.Id).Sum(x => x.TotalCost);

            if (utilisedBudget > employeeBudget)
            {
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = string.Format("Utilised Budget {0} is exceeding Employee Budget {1}.", utilisedBudget, employeeBudget) };
                return Json(data: new { Success = false, Message = string.Format("Utilised Budget {0} is exceeding Employee Budget {1}.", utilisedBudget, employeeBudget) }, JsonRequestBehavior.AllowGet);
            }
            
            OrderTblRow.OrderApproved = "Y";

            db.Entry(OrderTblRow).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("Order {0} approved.", OrderTblRow.OrderId) };
            return Json(data: new { Success = true, Message = "Order Approved" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reject(int OrderId)
        {
            var OrderTblRow = db.tblOrders.Where(x => x.Id == OrderId).Single();

            OrderTblRow.OrderApproved = "R";

            db.Entry(OrderTblRow).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("Order {0} rejected.", OrderTblRow.OrderId) };
            return Json(data: new { Success = true, Message = "Order Rejected" }, JsonRequestBehavior.AllowGet);
        }
    }
}