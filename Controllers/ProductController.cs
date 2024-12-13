//Bearbeiter: Abbas Dayeh(Warenkorb hinzufügen, Übersicht, Leihdauerauswahl, Artikel entfernen, Warenkorb leeren)
//            Alper Daglioglu(Warenkorb hinzufügen, Bestellung aufgeben, Warenkorb Artikel entfernen, Filterfunktion, Suchleiste)
//            Yusuf Can Sönmez(Index, Details, Filterfunktion)
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
using System.Globalization;

namespace WebShop.Controllers
{
    /// <summary>
    /// Der ProductController verwaltet die Produkte im WebShop.
    /// Er bietet Funktionen zum Anzeigen, Filtern, Hinzufügen zum Warenkorb, Bearbeiten des Warenkorbs, Entfernen von Artikeln, Anzeigen von Details, Bestellen und Suchen von Produkten.
    /// </summary>
    [Authorize]
    public class ProductController : Controller
    {
        int _userRole;
        private WebShopEntities db;
        List<ShoppingCartModel> lstShoppingCartModel;

        /// <summary>
        /// Konstruktor, der die Datenbankverbindung initialisiert und den Warenkorb initialisiert.
        /// </summary>
        public ProductController()
        {
            db = new WebShopEntities();
            lstShoppingCartModel = new List<ShoppingCartModel>();
        }

        /// <summary>
        /// Zeigt die Hauptseite der Produktverwaltung an.
        /// </summary>
        /// <returns>Die Index-Ansicht mit einer Liste von Produkten.</returns>
        public ActionResult Index(string searchString = "")
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            List<ProductModel> lstProducts = new List<ProductModel>();

            var query = db.tblItems.Include(x => x.tblStocks).Where(x => x.IsActive == "Y").Select(l => new ProductModel
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                ProductType = l.tblItemTypeMaster.ItemType,
                Cost = l.Cost,
                ImageName = l.ImageName,
                ItemsInStock = l.tblStocks.Sum(p => p.Quantity)
            });

            if (string.IsNullOrEmpty(searchString))
            {
                lstProducts = query.ToList();
            }
            else
            {
                lstProducts = query.Where(x => x.Name.Contains(searchString)).ToList();
                ViewBag.SearchString = searchString;
            }

            return View(lstProducts);
        }

        /// <summary>
        /// Filtert die Produkte nach dem angegebenen Filter (Hardware oder Software).
        /// </summary>
        /// <param name="filter">Der Filter, nach dem die Produkte gefiltert werden sollen.</param>
        /// <returns>Die gefilterte Liste von Produkten.</returns>
        public ActionResult IndexFilter(string filter)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            List<ProductModel> lstProducts = new List<ProductModel>();

            if (filter == "Hardware")
            {
                ViewBag.SelectedFilter = "Hardware";
                lstProducts = db.tblItems.Include(x => x.tblStocks)
                    .Where(x => x.IsActive == "Y")
                    .Where(x => x.tblItemTypeMaster.Id == (int)ItemTypeEnum.Hardware)
                    .Select(l => new ProductModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Description = l.Description,
                        ProductType = l.tblItemTypeMaster.ItemType,
                        Cost = l.Cost,
                        ImageName = l.ImageName,
                        ItemsInStock = l.tblStocks.Sum(p => p.Quantity)
                    }).ToList();
            }
            else
            {
                ViewBag.SelectedFilter = "Software";
                lstProducts = db.tblItems.Include(x => x.tblStocks)
                    .Where(x => x.IsActive == "Y")
                    .Where(x => x.tblItemTypeMaster.Id == (int)ItemTypeEnum.LicensedSoftware || x.tblItemTypeMaster.Id == (int)ItemTypeEnum.RentalSoftware)
                    .Select(l => new ProductModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Description = l.Description,
                        ProductType = l.tblItemTypeMaster.ItemType,
                        Cost = l.Cost,
                        ImageName = l.ImageName,
                        ItemsInStock = l.tblStocks.Sum(p => p.Quantity)
                    }).ToList();
            }

            return View("Index", lstProducts);
        }

        /// <summary>
        /// Fügt ein Produkt zum Warenkorb hinzu.
        /// </summary>
        /// <param name="ItemId">Die ID des hinzuzufügenden Produkts.</param>
        /// <returns>Eine JSON-Antwort, die den Erfolg des Hinzufügens anzeigt.</returns>
        [HttpPost]
        public JsonResult AddToCart(int ItemId)
        {
            ShoppingCartModel objShoppingCart = new ShoppingCartModel();
            var addedItem = db.tblItems.Single(x => x.Id == ItemId);

            if (Session["CartItem"] != null)
            {
                lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            }

            if (lstShoppingCartModel.Any(x => x.Id == ItemId))
            {
                // Wenn der Artikel bereits im Warenkorb ist, nichts tun
                // Mengenänderung verworfen
            }
            else
            {
                objShoppingCart.Id = ItemId;
                objShoppingCart.ImageName = addedItem.ImageName;
                objShoppingCart.Name = addedItem.Name;
                objShoppingCart.Type = addedItem.Type;
                objShoppingCart.LendingPeriodMonths = 1;
                objShoppingCart.LendingStartDt = DateTime.Now;
                objShoppingCart.LendingEndDt = DateTime.Now.AddMonths(1);
                objShoppingCart.CartQuantity = 1;
                objShoppingCart.UnitPrice = decimal.Parse(addedItem.Cost, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                objShoppingCart.Total = objShoppingCart.UnitPrice * objShoppingCart.LendingPeriodMonths;

                lstShoppingCartModel.Add(objShoppingCart);
            }

            Session["CartCounter"] = lstShoppingCartModel.Count;
            Session["CartItem"] = lstShoppingCartModel;
            return Json(data: new { Success = true, Counter = lstShoppingCartModel.Count }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Bearbeitet den Warenkorb, indem die Leihdauer eines Artikels geändert wird.
        /// </summary>
        /// <param name="ItemId">Die ID des zu bearbeitenden Artikels.</param>
        /// <param name="LendingPeriod">Die neue Leihdauer in Monaten.</param>
        /// <returns>Eine JSON-Antwort, die den Erfolg der Bearbeitung anzeigt.</returns>
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

        /// <summary>
        /// Zeigt den Warenkorb an.
        /// </summary>
        /// <returns>Die Ansicht des Warenkorbs.</returns>
        public ActionResult ShoppingCart()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            lstShoppingCartModel = Session["CartItem"] as List<ShoppingCartModel>;
            return View(lstShoppingCartModel);
        }

        /// <summary>
        /// Entfernt einen Artikel aus dem Warenkorb.
        /// </summary>
        /// <param name="ItemId">Die ID des zu entfernenden Artikels.</param>
        /// <returns>Leitet zur Warenkorbansicht weiter.</returns>
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

        /// <summary>
        /// Zeigt die Produktseite eines Produkts an.
        /// </summary>
        /// <param name = "id" > Die ID des anzuzeigenden Produkts.</param>
        /// <returns>Die Detailansicht des Produkts.</returns>
        public ActionResult DetailsById(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var selectedItem = db.tblItems.Where(x => x.Id == id).Single();
            return View("Details", selectedItem);
        }

        /// <summary>
        /// Zeigt die Details eines Produkts an.
        /// </summary>
        /// <param name="id">Die ID des anzuzeigenden Produkts.</param>
        /// <returns>Die Detailansicht des Produkts.</returns>
        public ActionResult Details(string name)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var selectedItem = db.tblItems.Where(x => x.Name.Contains(name)).FirstOrDefault();
            return View(selectedItem);
        }

        /// <summary>
        /// Platziert eine Bestellung mit den Artikeln im Warenkorb.
        /// </summary>
        /// <returns>Eine JSON-Antwort, die den Erfolg oder Fehler der Bestellung anzeigt.</returns>
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
                int orderedById = db.tblUsers.Where(x => x.UserName == currUser).Single().Id;

                //validation to check if budget is assigned for current year, need to check this because when team leader will approve the order
                //we have a validation to check if order value is greater than assigned budget

                if (_userRole == (int)UserRoleEnum.Employee)
                {
                    var empBudget = db.tblTeamEmployees.Where(x => x.TeamEmployeeId == orderedById && x.Year == DateTime.Now.Year).SingleOrDefault();
                    if (empBudget == null)
                    {
                        TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = string.Format("Budget Not Assigned For Year {0}", DateTime.Now.Year) };
                        return Json(data: new { Success = false, Message = string.Format("Budget Not Assigned For Year {0}", DateTime.Now.Year) }, JsonRequestBehavior.AllowGet);
                    }
                }

                tblOrder _tblOrder = new tblOrder();
                _tblOrder.OrderId = Guid.NewGuid().ToString();
                _tblOrder.OrderedBy = orderedById;
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

                    if (item.Type == (int)ItemTypeEnum.RentalSoftware) // Wenn der Artikel Mietsoftware ist
                    {
                        //_tblOrderDetail.StockDetailsId = null;
                        _tblOrderDetail.StockDetailsId = availableStockDetail.Id;
                        _tblOrderDetail.ItemId = item.Id;
                    }
                    else
                    {
                        _tblOrderDetail.ItemId = item.Id;
                        _tblOrderDetail.StockDetailsId = availableStockDetail.Id;
                    }

                    db.tblOrderDetails.Add(_tblOrderDetail);
                    if (availableStockDetail != null)
                    {
                        availableStockDetail.OrderId = generatedOrderId;
                        db.Entry(availableStockDetail).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }

                transaction.Commit();

                // Warenkorb aktualisieren
                Session["CartCounter"] = null;
                Session["CartItem"] = null;

                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Bestellung erfolgreich") };
                return Json(data: new { Success = true, Message = "Bestellung erfolgreich" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                transaction.Rollback();
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Fehler!", Message = "Bestellung fehlgeschlagen" };
                return Json(data: new { Success = false, Message = "Bestellung fehlgeschlagen" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Sucht nach Produkten anhand eines Suchbegriffs.
        /// </summary>
        /// <param name="SearchedString">Der Suchbegriff, nach dem gesucht werden soll.</param>
        /// <returns>Eine JSON-Antwort mit der Liste der gefundenen Produkte.</returns>
        public JsonResult Search(string SearchedString)
        {
            List<ProductModel> lstProducts = db.tblItems.Include(x => x.tblStocks).Where(x => x.IsActive == "Y")
                .Where(i => i.Name.Contains(SearchedString)).Select(l => new ProductModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    ProductType = l.tblItemTypeMaster.ItemType,
                    Cost = l.Cost,
                    ImageName = l.ImageName,
                    ItemsInStock = l.tblStocks.Sum(p => p.Quantity)
                }).ToList();

            return Json(lstProducts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearCart()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            Session["CartCounter"] = null;
            Session["CartItem"] = null;

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = "Warenkorb geleert." };
            return RedirectToAction("ShoppingCart");
        }
    }
}