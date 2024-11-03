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
using System.Web.Services.Description;

namespace WebShop.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        int _userRole;
        private WebShopEntities db = new WebShopEntities();
        
        public ActionResult ItemDetails()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            _ = new List<tblItem>();
            List<tblItem> lstItems = db.tblItems.Include(x => x.tblItemTypeMaster).ToList();
            return View(lstItems);
        }

        public ActionResult Add()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            return View();
        }

        [HttpPost]
        public ActionResult Add(ItemModel item)
        {

            tblItem newItemObj = new tblItem();
            newItemObj.Name = item.Name;
            newItemObj.Description = item.Description;
            newItemObj.Type = item.Type;
            newItemObj.Cost = item.Cost;

            if (item.ImageData != null)
            {
                var uniqueFileName = $@"{Guid.NewGuid()}" + "." + item.ImageData.ContentType.Substring(item.ImageData.ContentType.IndexOf("/") + 1);
                item.ImageData.SaveAs(Server.MapPath("~/Content/ItemImages") + "/" + uniqueFileName);
                newItemObj.ImageName = uniqueFileName;
            }

            db.tblItems.Add(newItemObj);
            db.SaveChanges();
            return RedirectToAction("ItemDetails");
        }

        public ActionResult Edit(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var selectedItem = db.tblItems.Where(x => x.Id == id).FirstOrDefault();
            ItemModel item = new ItemModel();
            item.Id = id;
            item.Name = selectedItem.Name;
            item.Description = selectedItem.Description;
            item.Type = selectedItem.Type;
            item.Cost = selectedItem.Cost;
            item.ImageName = selectedItem.ImageName;

            return View(item);
        }

        public ActionResult Deactivate(int id)
        {
            var selectedItem = db.tblItems.Include(x => x.tblStocks).Where(x => x.Id == id).Single();
            var itemsInStock = selectedItem.tblStocks.Sum(x => x.Quantity);

            if (itemsInStock == 0)
            {
                selectedItem.IsActive = "N";
                db.Entry(selectedItem).State = EntityState.Modified;
                db.SaveChanges();
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("Item {0} deleted.", selectedItem.Name) };
            }
            else
            {
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = itemsInStock + " Items availabe in stock. Item cannot be deleted." };
            } 

            return RedirectToAction("ItemDetails");
        }

        [HttpPost]
        public ActionResult Edit(ItemModel item)
        {
            var selectedItem = db.tblItems.Where(x => x.Id == item.Id).FirstOrDefault();

            selectedItem.Name = item.Name;
            selectedItem.Description = item.Description;
            selectedItem.Type = item.Type;
            selectedItem.Cost = item.Cost;

            if (item.ImageData != null)
            {
                var uniqueFileName = $@"{Guid.NewGuid()}" + "." + item.ImageData.ContentType.Substring(item.ImageData.ContentType.IndexOf("/") + 1);
                item.ImageData.SaveAs(Server.MapPath("~/Content/ItemImages") + "/" + uniqueFileName);
                selectedItem.ImageName = uniqueFileName;
            }

            db.Entry(selectedItem).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ItemDetails");
        }
    }
}