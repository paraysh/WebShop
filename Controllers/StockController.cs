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
            AddStockModel addStockModel = new AddStockModel();
            var itemInfo = db.tblItems.Where(x=> x.Id == id).Include(x => x.tblStocks).FirstOrDefault();

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
            if (addStockModel.LstSerialNumbers != null && addStockModel.LstSerialNumbers.Count > 0)
            {
                _tblStock.ItemId = addStockModel.Id;
                _tblStock.Quantity = addStockModel.Quantity;

                db.tblStocks.Add(_tblStock);
                db.SaveChanges();

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

            return RedirectToAction("StockDetails"); 
        }
    }
}