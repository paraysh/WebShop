using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

            var allItems = db.tblItems.Include(x => x.tblStocks).ToList();
            return View(allItems);
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

            addStockModel.Quantity = itemInfo.tblStocks.Count;
            return View(addStockModel); 
        }
    }
}