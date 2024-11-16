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
    /// <summary>
    /// Der StockController verwaltet den Bestand der Produkte im WebShop.
    /// Er bietet Funktionen zum Anzeigen, Hinzufügen, Entfernen und Anzeigen von Details des Bestands.
    /// </summary>
    [Authorize]
    public class StockController : Controller
    {
        int _userRole;
        private WebShopEntities db = new WebShopEntities();

        /// <summary>
        /// Zeigt die Hauptseite der Bestandsverwaltung an.
        /// </summary>
        /// <returns>Die Index-Ansicht.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Zeigt die Details des gesamten Bestands an.
        /// </summary>
        /// <returns>Die Ansicht mit der Liste aller Bestände.</returns>
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

        /// <summary>
        /// Zeigt die Ansicht zum Hinzufügen von Bestand für ein bestimmtes Produkt an.
        /// </summary>
        /// <param name="id">Die ID des Produkts, für das Bestand hinzugefügt werden soll.</param>
        /// <returns>Die Ansicht zum Hinzufügen von Bestand.</returns>
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

        /// <summary>
        /// Fügt den Bestand für ein bestimmtes Produkt hinzu.
        /// </summary>
        /// <param name="addStockModel">Das Modell, das die Bestandsinformationen enthält.</param>
        /// <returns>Eine JSON-Antwort, die den Erfolg des Hinzufügens anzeigt.</returns>
        [HttpPost]
        public ActionResult Add(AddStockModel addStockModel)
        {
            // Überprüft auf doppelte Seriennummern
            if (addStockModel.LstSerialNumbers != null && addStockModel.LstSerialNumbers.Count > 0)
            {
                var anyDuplicateInNewSerialNum = addStockModel.LstSerialNumbers.GroupBy(x => x).Any(g => g.Count() > 1);
                var anyDuplicateInDB = db.tblStockDetails.Where(x => addStockModel.LstSerialNumbers.Contains(x.SerialNumber)).Count() > 0;

                if (anyDuplicateInNewSerialNum || anyDuplicateInDB)
                {
                    return Json(data: new { Error = true, Message = "Doppelte Seriennummer erkannt." }, JsonRequestBehavior.AllowGet);
                }
            }

            tblStock _tblStock = new tblStock();
            _tblStock.ItemId = addStockModel.Id;
            _tblStock.Quantity = addStockModel.Quantity;
            _tblStock.CreatedBy = User.Identity.GetUserName();
            _tblStock.CreatedDate = DateTime.Now;
            _tblStock.InitialQuantity = addStockModel.Quantity;

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
                    _tblStockDetail.IsDeleted = "N";
                    db.tblStockDetails.Add(_tblStockDetail);
                }
                db.SaveChanges();
            }

            return Json(data: new { Success = true, Message = "{0} zum Bestand hinzugefügt." }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Zeigt die Ansicht zum Entfernen von Bestand für ein bestimmtes Produkt an.
        /// </summary>
        /// <param name="id">Die ID des Produkts, für das Bestand entfernt werden soll.</param>
        /// <returns>Die Ansicht zum Entfernen von Bestand.</returns>
        public ActionResult Remove(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            AddStockModel addStockModel = new AddStockModel();
            var itemInfo = db.tblItems.Where(x => x.Id == id).Single();

            var stockInfo = itemInfo.tblStocks.SelectMany(x => x.tblStockDetails).Where(x => x.OrderId == null && x.IsDeleted == "N").SelectMany(s => new List<SelectListItem> {
                new SelectListItem
                {
                    Text = s.SerialNumber,
                    Value = s.Id.ToString(),
                    Selected = false
                }
            }).ToList(); // Wählt nur die Seriennummern aus, deren OrderId == null ist

            addStockModel.Id = id;
            addStockModel.Name = itemInfo.Name;
            addStockModel.Description = itemInfo.Description;
            addStockModel.Type = itemInfo.Type;
            addStockModel.Cost = itemInfo.Cost;
            addStockModel.ImageName = itemInfo.ImageName;
            addStockModel.SelectLstSerialNumbers = stockInfo;

            addStockModel.Quantity = 0;
            return View(addStockModel);
        }

        /// <summary>
        /// Entfernt den Bestand für ein bestimmtes Produkt.
        /// </summary>
        /// <param name="stockModel">Das Modell, das die Bestandsinformationen enthält.</param>
        /// <returns>Leitet zur Bestandsdetailansicht weiter.</returns>
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

            stockDetailModel.IsDeleted = "Y";
            stockDetailModel.DeleteReason = stockModel.DeleteReason;
            db.Entry(stockDetailModel).State = EntityState.Modified;
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Artikel {0} entfernt.", stockDetailModel.SerialNumber) };
            return RedirectToAction("StockDetails");
        }

        /// <summary>
        /// Zeigt die Details eines bestimmten Produkts an.
        /// </summary>
        /// <param name="id">Die ID des anzuzeigenden Produkts.</param>
        /// <returns>Die Detailansicht des Produkts.</returns>
        public ActionResult Details(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var tblItem = db.tblItems
                .Include(x => x.tblStocks)
                .Include(x => x.tblOrderDetails)
                .Where(x => x.Id == id)
                .Select(item => new StockHistoryModel
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    ItemCost = (decimal)item.Cost,
                    ItemImageName = item.ImageName,
                    ItemType = item.tblItemTypeMaster.ItemType,
                    lstStocks = item.tblStocks.SelectMany(stock => new List<Stock> { new Stock {
                        InitialQuantity = (int)stock.InitialQuantity,
                        StockCurrentQuantity = (int)stock.Quantity,
                        StockAddDate = (DateTime)stock.CreatedDate,
                        StockAddedBy = stock.CreatedBy,
                        StockDeletedBy = stock.ModifiedBy == null ? "NA" : stock.ModifiedBy,
                        StockDeleteDate = stock.ModifiedDt,
                        lstStockDetails = stock.tblStockDetails.SelectMany(stockDtl => new List<StockDetail> { new StockDetail {
                            SerialNo = stockDtl.SerialNumber,
                            OrderId = stockDtl.OrderId,
                            IsDeleted = stockDtl.IsDeleted,
                            OrderID = stockDtl.tblOrder.OrderId,
                            OrderedBy = stockDtl.tblOrder.tblUser.UserName,
                            OrderDate = (DateTime)stockDtl.tblOrder.OrderDate,
                            OrderApproved = stockDtl.tblOrder.OrderApproved,
                            LendingPeriodMonths = stockDtl.tblOrderDetails.Where(o => o.StockDetailsId == stockDtl.Id).FirstOrDefault() == null ? 0 : (int)stockDtl.tblOrderDetails.Where(o => o.StockDetailsId == stockDtl.Id).FirstOrDefault().LendingPeriodMonths,
                            LendingStartDt = stockDtl.tblOrderDetails.Where(o => o.StockDetailsId == stockDtl.Id).FirstOrDefault() == null ? null : stockDtl.tblOrderDetails.Where(o => o.StockDetailsId == stockDtl.Id).FirstOrDefault().LendingStartDt,
                            LendingEndDt = stockDtl.tblOrderDetails.Where(o => o.StockDetailsId == stockDtl.Id).FirstOrDefault() == null ? null : stockDtl.tblOrderDetails.Where(o => o.StockDetailsId == stockDtl.Id).FirstOrDefault().LendingEndDt
                                //lstOrderDetails = stockDtl.tblOrderDetails.SelectMany(orderDtl => new List<OrderDetails> { new OrderDetails {
                                //        OrderID = orderDtl.tblOrder.OrderId,
                                //        OrderedBy = orderDtl.tblOrder.tblUser.UserName,
                                //        OrderDate = (DateTime)orderDtl.tblOrder.OrderDate,
                                //        OrderApproved = orderDtl.tblOrder.OrderApproved,
                                //        LendingPeriodMonths = (int)orderDtl.LendingPeriodMonths,
                                //        LendingStartDt = (DateTime)orderDtl.LendingStartDt,
                                //        LendingEndDt = (DateTime)orderDtl.LendingEndDt
                                //} }).ToList()
                        } }).ToList()
                    } }).ToList()
                }).Single();

            return View(tblItem);
        }
    }
}