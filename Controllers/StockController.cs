using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Models.Enum;

namespace WebShop.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        int _userRole;
        private WebShopEntities db = new WebShopEntities();
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StockDetails()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var allItems = db.tblItems.Include(x => x.tblStocks).Select(x => new AddStockModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ItemTypeStr = x.tblItemTypeMaster.ItemType,
                ImageName = x.ImageName,
                Cost = x.Cost,
                Quantity = x.tblStocks.Where(i => i.ItemId == x.Id).Count() == 0 ? 0 : (int)x.tblStocks.Where(i => i.ItemId == x.Id).Sum(s => s.Quantity),
            });
            return View(allItems.ToList());
        }

        public ActionResult Add(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            AddStockModel addStockModel = new AddStockModel();
            var itemInfo = db.tblItems.Where(x => x.Id == id).Include(x => x.tblStocks).FirstOrDefault();

            addStockModel.Id = id;
            addStockModel.Name = itemInfo.Name;
            addStockModel.Description = itemInfo.Description;
            addStockModel.Type = itemInfo.Type;
            addStockModel.Cost = itemInfo.Cost;
            addStockModel.ImageName = itemInfo.ImageName;

            addStockModel.Quantity = 0;
            return View(addStockModel);
        }

        [HttpPost]
        public ActionResult Add(AddStockModel addStockModel)
        {
            tblStock _tblStock = new tblStock();
            _tblStock.ItemId = addStockModel.Id;
            _tblStock.Quantity = addStockModel.Quantity;

            db.tblStocks.Add(_tblStock);
            db.SaveChanges();

            if (addStockModel.LstSerialNumbers != null && addStockModel.LstSerialNumbers.Count > 0)
            {
                int generatedStockId = _tblStock.Id;

                foreach (var item in addStockModel.LstSerialNumbers)
                {
                    tblStockDetail _tblStockDetail = new tblStockDetail();
                    _tblStockDetail.SerialNumber = item;
                    _tblStockDetail.StockId = generatedStockId;
                    db.tblStockDetails.Add(_tblStockDetail);
                }
                db.SaveChanges();
            }

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("{0} added from stock.", addStockModel.Name) };
            return RedirectToAction("StockDetails");
        }

        public ActionResult Remove(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            AddStockModel addStockModel = new AddStockModel();
            //List<string> stockInfo = new List<string>();
            var itemInfo = db.tblItems.Where(x => x.Id == id).Single();

            var stockInfo = itemInfo.tblStocks.SelectMany(x => x.tblStockDetails).Where(x => x.OrderId == null).SelectMany(s => new List<SelectListItem> {
                new SelectListItem
                {
                    Text = s.SerialNumber,
                    Value = s.Id.ToString(),
                    Selected = false
                }
                
            }).ToList(); // select only those serial number whose OrderId == null

            addStockModel.Id = id;
            addStockModel.Name = itemInfo.Name;
            addStockModel.Description = itemInfo.Description;
            addStockModel.Type = itemInfo.Type;
            addStockModel.Cost = itemInfo.Cost;
            addStockModel.ImageName = itemInfo.ImageName;
            //addStockModel.LstSerialNumbers = stockInfo;
            addStockModel.SelectLstSerialNumbers = stockInfo;

            addStockModel.Quantity = 0;
            return View(addStockModel);
        }


        [HttpPost]
        public ActionResult Remove(AddStockModel stockModel)
        {
            int stockDetailsId = Convert.ToInt32(stockModel.SelectedSerialNo);
            var stockDetailModel = db.tblStockDetails.Where(x => x.Id == stockDetailsId).Single();

            var tblStockModel = db.tblStocks.Where(x => x.Id == stockDetailModel.StockId).Single();
            tblStockModel.Quantity = tblStockModel.Quantity - 1;
            tblStockModel.ModifiedBy = User.Identity.GetUserName();
            tblStockModel.ModifiedDt = DateTime.Now;

            db.Entry(tblStockModel).State = EntityState.Modified;
            db.tblStockDetails.Remove(stockDetailModel);
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("{0} removed from stock.", stockDetailModel.SerialNumber) };
            return RedirectToAction("StockDetails");
        }
    }
}