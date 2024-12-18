//Bearbeiter: Abbas Dayeh(Details V2)
//            Alper Daglioglu(Details V1, Artikel entfernen Funktionen)
//            Yusuf Can Sönmez(V1 von Details, Artikel ein-/ ausgang Funktionen, BaseController Erweiterung)
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
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
    public class StockController : BaseController
    {
        int _userRole;
        private WebShopEntities db = new WebShopEntities();
        ClaimsPrincipal prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;

        public StockController()
        {
                
        }
        public StockController(WebShopEntities _db) : base(_db)
        {
            db = _db; 
        }

        /// <summary>
        /// Zeigt die Details des gesamten Bestands an.
        /// </summary>
        /// <returns>Die Ansicht mit der Liste aller Bestände.</returns>
        public ActionResult Index()
        {
            // Setzt die Benutzerrolle
            _userRole = Convert.ToInt32(prinicpal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            ViewBag.UserRole = _userRole;

            // Holt alle Artikel und deren Bestände aus der Datenbank
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
            // Setzt die Benutzerrolle
            _userRole = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
            ViewBag.UserRole = _userRole;

            // Holt die Informationen des Artikels aus der Datenbank
            AddStockModel addStockModel = new AddStockModel();
            var itemInfo = db.tblItems.Where(x => x.Id == id).Include(x => x.tblStocks).FirstOrDefault();

            // Setzt die Informationen des Artikels im Modell
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

            if (addStockModel.Quantity == 0)
            {
                return Json(data: new { Error = true, Message = "Ungültige Menge." }, JsonRequestBehavior.AllowGet);
            }

            // Erstellt einen neuen Bestandseintrag
            tblStock _tblStock = new tblStock();
            _tblStock.ItemId = addStockModel.Id;
            _tblStock.Quantity = addStockModel.Quantity;
            _tblStock.CreatedBy = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Name)
                                    .Select(c => c.Value)
                                    .SingleOrDefault();
            _tblStock.CreatedDate = DateTime.Now;
            _tblStock.InitialQuantity = addStockModel.Quantity;

            db.tblStocks.Add(_tblStock);
            db.SaveChanges();

            int generatedStockId = _tblStock.Id;

            // Fügt die Seriennummern zum Bestand hinzu
            if (addStockModel.LstSerialNumbers != null && addStockModel.LstSerialNumbers.Count > 0)
            {
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
            else // Für Software
            {
                for (int i = 0; i < addStockModel.Quantity; i++)
                {
                    tblStockDetail _tblStockDetail = new tblStockDetail();
                    _tblStockDetail.SerialNumber = null;
                    _tblStockDetail.StockId = generatedStockId;
                    _tblStockDetail.IsDeleted = "N";
                    db.tblStockDetails.Add(_tblStockDetail);
                }
                db.SaveChanges();
            }

            return Json(data: new { Success = true, Message = "Zum Bestand hinzugefügt." }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Zeigt die Ansicht zum Entfernen von Bestand für ein bestimmtes Produkt an.
        /// </summary>
        /// <param name="id">Die ID des Produkts, für das Bestand entfernt werden soll.</param>
        /// <returns>Die Ansicht zum Entfernen von Bestand.</returns>
        public ActionResult Remove(int id)
        {
            // Setzt die Benutzerrolle
            _userRole = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
            ViewBag.UserRole = _userRole;

            // Holt die Informationen des Artikels aus der Datenbank
            AddStockModel addStockModel = new AddStockModel();
            var itemInfo = db.tblItems.Where(x => x.Id == id).Single();

            // Holt die Seriennummern des Bestands, die entfernt werden können
            var stockInfo = itemInfo.tblStocks.SelectMany(x => x.tblStockDetails).Where(x => x.OrderId == null && x.IsDeleted == "N").SelectMany(s => new List<SelectListItem> {
                new SelectListItem
                {
                    Text = s.SerialNumber,
                    Value = s.Id.ToString(),
                    Selected = false
                }
            }).ToList(); // Wählt nur die Seriennummern aus, deren OrderId == null ist

            // Setzt die Informationen des Artikels im Modell
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
            var itemObj = db.tblItems.Include(x => x.tblStocks).Where(x => x.Id == stockModel.Id).Single();
            if (itemObj.Type == (int)ItemTypeEnum.RentalSoftware)
            {
                if (stockModel.Quantity <= 0)
                {
                    TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Fehler!", Message = "Ungültige Menge." };
                    return RedirectToAction("Index");
                }

                var totalQuantity = itemObj.tblStocks.Sum(x => x.Quantity);
                if (stockModel.Quantity > totalQuantity)
                {
                    TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Fehler!", Message = string.Format("Die verfügbare Menge {0} ist kleiner als die Löschmenge.", totalQuantity) };
                    return RedirectToAction("Index");
                }

                var objTblStock = itemObj.tblStocks.Where(x => x.Quantity > 0).FirstOrDefault();
                objTblStock.Quantity = objTblStock.Quantity - stockModel.Quantity;
                objTblStock.ModifiedBy = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
                objTblStock.ModifiedDt = DateTime.Now;

                db.SetModified(objTblStock);

                var lstTblStockDtls = objTblStock.tblStockDetails.Where(x => x.IsDeleted == "N" && x.OrderId == null).Take(stockModel.Quantity).ToList();
                foreach (var objTblStockDtls in lstTblStockDtls)
                {
                    objTblStockDtls.IsDeleted = "Y";
                    objTblStockDtls.DeleteReason = stockModel.DeleteReason;
                    db.SetModified(objTblStockDtls);
                }
                
                db.SaveChanges();

                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Artikel entfernt.") };
                return RedirectToAction("Index");
            }

            // Holt die Bestandsdetails aus der Datenbank
            int stockDetailsId = Convert.ToInt32(stockModel.SelectedSerialNo);
            var stockDetailModel = db.tblStockDetails.Where(x => x.Id == stockDetailsId).Single();

            // Holt den Bestandseintrag aus der Datenbank und aktualisiert die Menge
            var tblStockModel = db.tblStocks.Where(x => x.Id == stockDetailModel.StockId).Single();
            tblStockModel.Quantity = tblStockModel.Quantity - 1;
            tblStockModel.ModifiedBy = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            tblStockModel.ModifiedDt = DateTime.Now;

            db.SetModified(tblStockModel);

            // Markiert die Bestandsdetails als gelöscht
            stockDetailModel.IsDeleted = "Y";
            stockDetailModel.DeleteReason = stockModel.DeleteReason;
            db.SetModified(stockDetailModel);
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Artikel {0} entfernt.", stockDetailModel.SerialNumber) };
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Zeigt die Details eines bestimmten Produkts an.
        /// </summary>
        /// <param name="id">Die ID des anzuzeigenden Produkts.</param>
        /// <returns>Die Detailansicht des Produkts.</returns>
        public ActionResult DetailsV2(int id, string itemName = "")
        {
            // Setzt die Benutzerrolle
            _userRole = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault());
            ViewBag.UserRole = _userRole;
            ViewBag.ItemName = itemName;

            var counts = new List<int>();

            StockHistoryModel model = new StockHistoryModel();
            List<Stock> stocks = new List<Stock>();
            List<StockDetail> stockDetail = new List<StockDetail>();

            // Holt die hinzugefügten Bestände aus der Datenbank
            var stockAdded = db.tblStockDetails
                .Include(x => x.tblStock)
                .Where(x => x.tblStock.ItemId == id)
                .GroupBy(x => new { x.tblStock.CreatedBy, CreatedDate = SqlFunctions.DateName("dd", x.tblStock.CreatedDate.Value) + " " + SqlFunctions.DateName("MM", x.tblStock.CreatedDate.Value) + " " + SqlFunctions.DateName("yyyy", x.tblStock.CreatedDate.Value) })
                .Select(item => new Stock
                {
                    StockAddDate = item.Key.CreatedDate,
                    //TotalItemsAddedRemoved = item.Select(x => new SerialNumbers { SerialNos = x.SerialNumber }).ToList().Count,
                    StockAddedBy = item.Key.CreatedBy,
                    MovementType = "Eingang", // Added
                    lstSerialNumbers = item.Select(x => new SerialNumbers { SerialNos = x.SerialNumber, DeleteReason = "Eingang" }).ToList()
                })
                .ToList();

            // Holt die entfernten Bestände aus der Datenbank
            var stockDeleted = db.tblStockDetails
                .Include(x => x.tblStock)
                .Where(x => x.tblStock.ItemId == id && x.tblStock.ModifiedBy != null)
                .GroupBy(x => new { x.tblStock.CreatedBy, CreatedDate = SqlFunctions.DateName("dd", x.tblStock.CreatedDate.Value) + " " + SqlFunctions.DateName("MM", x.tblStock.CreatedDate.Value) + " " + SqlFunctions.DateName("yyyy", x.tblStock.CreatedDate.Value) })
                .Select(item => new Stock
                {
                    StockAddDate = item.Key.CreatedDate,
                    StockAddedBy = item.Key.CreatedBy,
                    MovementType = "Ausgang", // Deleted
                    lstSerialNumbers = item.Where(x => x.IsDeleted == "Y").Select(x => new SerialNumbers { SerialNos = x.SerialNumber, DeleteReason = x.DeleteReason }).ToList(),
                })
                .ToList();

            // Fügt die hinzugefügten und entfernten Bestände zur Liste hinzu
            stocks.AddRange(stockAdded);
            stocks.AddRange(stockDeleted);

            // Sortiert die Bestände nach Datum
            stocks = stocks.OrderBy(x => x.StockAddDate).ToList();

            model.lstStocks = stocks;

            // Holt die Bestelldetails aus der Datenbank
            var orderDetails = db.tblStockDetails
                .Include(x => x.tblStock)
                .Include(x => x.tblOrderDetails)
                .Where(x => x.tblStock.ItemId == id && x.OrderId != null)
                .Select(item => new StockDetail
                {
                    SerialNo = item.SerialNumber,
                    OrderedBy = item.tblOrder.tblUser.UserName,
                    LendingStartDt = item.tblOrderDetails.Where(x => x.StockDetailsId == item.Id).Select(x => x.LendingStartDt).FirstOrDefault(),
                    LendingEndDt = item.tblOrderDetails.Where(x => x.StockDetailsId == item.Id).Select(x => x.LendingEndDt).FirstOrDefault()
                }).ToList();

            model.lstStockDetails = orderDetails;

            // Holt die im Bestand befindlichen Artikel aus der Datenbank
            model.lstInStockItems = db.tblStockDetails
                                    .Include(x => x.tblStock)
                                    .Where(x => x.tblStock.ItemId == id && x.OrderId == null && x.IsDeleted == "N")
                                    .Select(x => x.SerialNumber).ToList();

            return View(model);
        }
    }
}