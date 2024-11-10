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
using System.Security.Claims;
using System.Threading;
using Microsoft.Ajax.Utilities;

namespace WebShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        int _userRole;
        private WebShopEntities db;
        ClaimsPrincipal prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;

        public OrderController()
        {
            db = new WebShopEntities();
        }
        public ActionResult Index()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            int userID = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            ViewBag.UserId = userID;

            List<OrderModel> model = new List<OrderModel>();

            var lstOrders = db.tblOrders.Include(x => x.tblOrderDetails).Include(x => x.tblStockDetails).Include(x => x.tblUser).SelectMany(x => new List<OrderModel> {
                new OrderModel
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    OrderedBy = (int)x.OrderedBy,
                    OrderedByName = x.tblUser.UserName,
                    OrderDt = x.OrderDate.Value,
                    OrderApproved = x.OrderApproved,
                    OrderStatus = x.OrderApproved == "R" ? "Abgelehnt" : (x.OrderApproved == "Y" ? "Genehmigt" : "Ausstehend"),
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
            });

            if (_userRole == (int)UserRoleEnum.TeamLeaders)
            {
                int currUserID = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
                List<int> employeeList = new List<int>() { currUserID };
                var teamEmployees = db.tblTeamEmployees.Where(x => x.TeamLeaderId == currUserID).Select(x => x.tblUser.Id).ToList();
                employeeList.AddRange(teamEmployees);
                var filteredList = lstOrders.Where(x => employeeList.Contains(x.OrderedBy)).ToList();
                return View(filteredList);
            }
            else
            {
                int currUserID = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
                var filteredList = lstOrders.Where(x => x.OrderedBy == currUserID).ToList();
                return View(filteredList);
            }
            //var lstStockDetails = lstOrders.SelectMany(x => x.tblStockDetails).ToList();
            //return View(lstOrders.ToList());
        }

        public ActionResult Approve(int OrderId)
        {
            var OrderTblRow = db.tblOrders.Where(x => x.Id == OrderId).Single();

            var currUserName = User.Identity.GetUserName();
            var empBudget = db.tblTeamEmployees.Where(x => x.TeamEmployeeId == OrderTblRow.OrderedBy).Single().TeamEmployeeBudget;
            var utilisedBudget = db.tblOrders.Where(x => x.OrderedBy == OrderTblRow.OrderedBy && x.OrderApproved != "R").Sum(x => x.TotalCost);

            if (utilisedBudget > empBudget)
            {
                //TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = string.Format("Utilised Budget {0} is exceeding Employee Budget {1}.", utilisedBudget, empBudget) };
                return Json(data: new { Error = true, Message = string.Format("Verwendetes Budget {0} überschreitet das Mitarbeiterbudget von {1}.", utilisedBudget, empBudget) }, JsonRequestBehavior.AllowGet);
            }
            
            OrderTblRow.OrderApproved = "Y";

            db.Entry(OrderTblRow).State = EntityState.Modified;
            db.SaveChanges();

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Bestellung {0} genehmigt.", OrderTblRow.OrderId) };
            return Json(data: new { Success = true, Message = string.Format("Bestellung {0} genehmigt.", OrderTblRow.OrderId) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reject(int OrderId)
        {
            var OrderTblRow = db.tblOrders.Include(x => x.tblStockDetails).Where(x => x.Id == OrderId).Single();
            OrderTblRow.OrderApproved = "R";

            OrderTblRow.tblStockDetails.ForEach(x => x.OrderId = null);
            OrderTblRow.tblStockDetails.ForEach(x => x.tblStock.Quantity = x.tblStock.Quantity + 1);

            db.Entry(OrderTblRow).State = EntityState.Modified;
            db.SaveChanges();

            // Free up stock


            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Bestellung {0} abgelehnt.", OrderTblRow.OrderId) };
            return Json(data: new { Success = true, Message = "Order Rejected" }, JsonRequestBehavior.AllowGet);
        }
    }
}