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
            List<ProductModel> lstProducts = db.tblItems.Include(x => x.tblStocks).Where(x => x.IsActive == "Y").Select(l => new ProductModel
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                ProductType = l.tblItemTypeMaster.ItemType,
                Cost = l.Cost,
                ImageName = l.ImageName,
                ItemsInStock = l.tblStocks.Sum(p => p.Quantity)

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
                //If item already added to cart, do nothing
                //Quantity increase discarded
            }
            else
            {
                objShoppingCart.Id = ItemId;
                objShoppingCart.ImageName = addedItem.ImageName;
                objShoppingCart.Name = addedItem.Name;
                objShoppingCart.Type = addedItem.Type;
                objShoppingCart.LendingPeriodMonths = addedItem.Type == (int)ItemTypeEnum.RentalSoftware ? 1 : 24;
                objShoppingCart.LendingStartDt = DateTime.Now;
                objShoppingCart.LendingEndDt = addedItem.Type == (int)ItemTypeEnum.RentalSoftware ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMonths(24);
                objShoppingCart.CartQuantity = 1;
                objShoppingCart.UnitPrice = addedItem.Cost.Value;
                objShoppingCart.Total = objShoppingCart.UnitPrice * objShoppingCart.LendingPeriodMonths;
                
                lstShoppingCartModel.Add(objShoppingCart);
            }

            Session["CartCounter"] = lstShoppingCartModel.Count;
            Session["CartItem"] = lstShoppingCartModel;
            return Json(data: new { Success = true, Counter = lstShoppingCartModel.Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditCart(int ItemId, int LendingPeriod)
        {
            ShoppingCartModel objShoppingCart = new ShoppingCartModel();

            if (Session["CartItem"] != null)
            {
                lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            }

            objShoppingCart = lstShoppingCartModel.Single(x => x.Id == ItemId);
            objShoppingCart.LendingPeriodMonths = LendingPeriod;
            objShoppingCart.LendingStartDt = DateTime.Now;
            objShoppingCart.LendingEndDt = DateTime.Now.AddMonths(LendingPeriod);
            objShoppingCart.Total = objShoppingCart.UnitPrice * objShoppingCart.LendingPeriodMonths;

            Session["CartCounter"] = lstShoppingCartModel.Count;
            Session["CartItem"] = lstShoppingCartModel;

            return Json(data: new { Success = true, Counter = lstShoppingCartModel.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShoppingCart()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            return View(lstShoppingCartModel);
        }

        public ActionResult RemoveItem(int ItemId)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            var selectedItem = lstShoppingCartModel.Single(x => x.Id == ItemId);
            lstShoppingCartModel.Remove(selectedItem);

            Session["CartCounter"] = lstShoppingCartModel.Count;
            Session["CartItem"] = lstShoppingCartModel;

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("{0} wurde aus dem Warenkorb entfernt.", selectedItem.Name) };

            return RedirectToAction("ShoppingCart");
        }

        public ActionResult Details(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var selectedItem = db.tblItems.Where(x => x.Id == id).Single();
            return View(selectedItem);
        }


        [HttpPost]
        public ActionResult PlaceOrder()
        {
            var transaction = db.Database.BeginTransaction();
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            var currUser = User.Identity.GetUserName();
            lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            try
            {
                tblOrder _tblOrder = new tblOrder();
                _tblOrder.OrderId = Guid.NewGuid().ToString();
                _tblOrder.OrderedBy = db.tblUsers.Where(x => x.UserName == currUser).Single().Id;
                _tblOrder.OrderDate = DateTime.Now;
                _tblOrder.OrderApproved = _userRole == (int)UserRoleEnum.Employee ? "N" : "Y";
                _tblOrder.TotalCost = lstShoppingCartModel.Sum(x => x.Total);
                _tblOrder.TotalItems = lstShoppingCartModel.Count();

                db.tblOrders.Add(_tblOrder);
                db.SaveChanges();


                int generatedOrderId = _tblOrder.Id;

                foreach (var item in lstShoppingCartModel)
                {
                    tblOrderDetail _tblOrderDetail = new tblOrderDetail();
                    _tblOrderDetail.OrderId = generatedOrderId;
                    _tblOrderDetail.LendingPeriodMonths = item.LendingPeriodMonths;
                    _tblOrderDetail.LendingStartDt = item.LendingStartDt;
                    _tblOrderDetail.LendingEndDt = item.LendingEndDt;

                    var itemInfo = db.tblItems.Where(x => x.Id == item.Id).Single();
                    var availableStock = itemInfo.tblStocks.Where(x => x.Quantity > 0).First();
                    var availableStockDetail = availableStock.tblStockDetails.Where(x => x.StockId == availableStock.Id && x.OrderId == null && x.IsDeleted == "N").FirstOrDefault();

                    availableStock.Quantity = availableStock.Quantity - 1;
                    db.Entry(availableStock).State = EntityState.Modified;

                    if (item.Type == (int)ItemTypeEnum.RentalSoftware) // When item is Rental Software
                    {
                        _tblOrderDetail.StockDetailsId = null;
                        _tblOrderDetail.ItemId = item.Id;
                    }
                    else
                        _tblOrderDetail.StockDetailsId = availableStockDetail.Id;
                   
                    
                    db.tblOrderDetails.Add(_tblOrderDetail);
                    if (availableStockDetail != null) 
                    {
                        availableStockDetail.OrderId = generatedOrderId;
                        db.Entry(availableStockDetail).State = EntityState.Modified;
                    }
                   
                    db.SaveChanges();
                }

                //Check if sofware is added for added hardware article

                transaction.Commit();

                //refresh cart
                Session["CartCounter"] = null;
                Session["CartItem"] = null;

                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Bestellung erfolgreich") };
                return Json(data: new { Success = true, Message = "Bestellung erfolgreich" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                transaction.Rollback();
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = "Order Failed!" };
                return Json(data: new { Success = false, Message = "Something went wrong" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}