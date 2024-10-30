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
    public class ProductController : Controller
    {
        int _userRole;
        private WebShopEntities db;
        List<ShoppingCartModel> lstShoppingCartModel;

        public ProductController()
        {
            db = new WebShopEntities();
            lstShoppingCartModel = new List<ShoppingCartModel>();
            
        }

        // GET: Product
        public ActionResult Index()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            _ = new List<ProductModel>();
            List<ProductModel> lstProducts = db.tblItems.Include(x => x.tblStocks).Where(s => s.tblStocks.Sum(q => q.Quantity) > 0).Select(l => new ProductModel
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                Cost = l.Cost,
                ImageName = l.ImageName
            }).ToList();

            return View(lstProducts);
        }

        [HttpPost]
        public JsonResult AddToCart(int ItemId) 
        {
            ShoppingCartModel objShoppingCart = new ShoppingCartModel();
            var addedItem = db.tblItems.Single(x => x.Id == ItemId);

            if (Session["CartItem"] != null) {
                lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            }

            if (lstShoppingCartModel.Any(x => x.Id == ItemId))
            {
                objShoppingCart = lstShoppingCartModel.Single(x => x.Id == ItemId);
                objShoppingCart.CartQuantity = objShoppingCart.CartQuantity + 1;
                objShoppingCart.Total = objShoppingCart.CartQuantity * objShoppingCart.UnitPrice;
            }
            else
            {
                objShoppingCart.Id = ItemId;
                objShoppingCart.ImageName = addedItem.ImageName;
                objShoppingCart.Name = addedItem.Name;
                objShoppingCart.CartQuantity = 1;
                objShoppingCart.Total = addedItem.Cost.Value;
                objShoppingCart.UnitPrice = addedItem.Cost.Value;
                lstShoppingCartModel.Add(objShoppingCart);
            }

            Session["CartCounter"] = lstShoppingCartModel.Count;
            Session["CartItem"] = lstShoppingCartModel;

            return Json(data: new { Success = true, Counter = lstShoppingCartModel.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShoppingCart()
        {
            lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            return View(lstShoppingCartModel);
        }
    }
}