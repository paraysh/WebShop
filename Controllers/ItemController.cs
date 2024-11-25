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
    /// <summary>
    /// Diese Klasse verwaltet die Artikel im WebShop.
    /// Sie ermöglicht das Anzeigen, Hinzufügen, Bearbeiten, Aktivieren und Deaktivieren von Artikeln.
    /// </summary>
    [Authorize]
    public class ItemController : Controller
    {
        // Speichert die Rolle des Benutzers
        int _userRole;
        // Datenbankkontext für den Zugriff auf die Datenbank
        private WebShopEntities db = new WebShopEntities();

        /// <summary>
        /// Zeigt die Details aller Artikel an.
        /// </summary>
        /// <returns>Eine Ansicht mit der Liste der Artikel.</returns>
        public ActionResult ItemDetails()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            _ = new List<tblItem>();
            // Holt alle Artikel und deren Typinformationen
            List<tblItem> lstItems = db.tblItems.Include(x => x.tblItemTypeMaster).ToList();
            return View(lstItems);
        }

        /// <summary>
        /// Zeigt die Seite zum Hinzufügen eines neuen Artikels an.
        /// </summary>
        /// <returns>Eine Ansicht zum Hinzufügen eines neuen Artikels.</returns>
        public ActionResult Add()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            return View();
        }

        /// <summary>
        /// Fügt einen neuen Artikel hinzu.
        /// </summary>
        /// <param name="item">Das Modell des hinzuzufügenden Artikels.</param>
        /// <returns>Leitet zur Artikeldetailseite weiter.</returns>
        [HttpPost]
        public ActionResult Add(ItemModel item)
        {
            // Erstellt ein neues Artikelobjekt und füllt es mit den übergebenen Daten
            tblItem newItemObj = new tblItem();
            newItemObj.Name = item.Name;
            newItemObj.Description = item.Description;
            newItemObj.Type = item.Type;
            newItemObj.Cost = item.Cost;
            newItemObj.IsActive = item.IsActive;

            // Speichert das Bild des Artikels, falls vorhanden
            if (item.ImageData != null)
            {
                var uniqueFileName = $@"{Guid.NewGuid()}" + "." + item.ImageData.ContentType.Substring(item.ImageData.ContentType.IndexOf("/") + 1);
                item.ImageData.SaveAs(Server.MapPath("~/Content/ItemImages") + "/" + uniqueFileName);
                newItemObj.ImageName = uniqueFileName;
            }

            // Fügt den neuen Artikel zur Datenbank hinzu und speichert die Änderungen
            db.tblItems.Add(newItemObj);
            db.SaveChanges();
            return RedirectToAction("ItemDetails");
        }

        /// <summary>
        /// Zeigt die Seite zum Bearbeiten eines Artikels an.
        /// </summary>
        /// <param name="id">Die ID des zu bearbeitenden Artikels.</param>
        /// <returns>Eine Ansicht zum Bearbeiten eines Artikels.</returns>
        public ActionResult Edit(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            // Holt den ausgewählten Artikel aus der Datenbank
            var selectedItem = db.tblItems.Where(x => x.Id == id).FirstOrDefault();
            ItemModel item = new ItemModel();
            item.Id = id;
            item.Name = selectedItem.Name;
            item.Description = selectedItem.Description;
            item.Type = selectedItem.Type;
            item.Cost =  selectedItem.Cost;
            item.ImageName = selectedItem.ImageName;

            return View(item);
        }

        /// <summary>
        /// Bearbeitet einen bestehenden Artikel.
        /// </summary>
        /// <param name="item">Das Modell des zu bearbeitenden Artikels.</param>
        /// <returns>Leitet zur Artikeldetailseite weiter.</returns>
        [HttpPost]
        public ActionResult Edit(ItemModel item)
        {
            // Holt den ausgewählten Artikel aus der Datenbank
            var selectedItem = db.tblItems.Where(x => x.Id == item.Id).FirstOrDefault();

            // Aktualisiert die Artikelinformationen
            selectedItem.Name = item.Name;
            selectedItem.Description = item.Description;
            selectedItem.Type = item.Type;
            selectedItem.Cost = item.Cost;

            // Speichert das neue Bild des Artikels, falls vorhanden
            if (item.ImageData != null)
            {
                var uniqueFileName = $@"{Guid.NewGuid()}" + "." + item.ImageData.ContentType.Substring(item.ImageData.ContentType.IndexOf("/") + 1);
                item.ImageData.SaveAs(Server.MapPath("~/Content/ItemImages") + "/" + uniqueFileName);
                selectedItem.ImageName = uniqueFileName;
            }

            // Markiert den Artikel als geändert und speichert die Änderungen in der Datenbank
            db.Entry(selectedItem).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ItemDetails");
        }

        /// <summary>
        /// Deaktiviert einen Artikel.
        /// </summary>
        /// <param name="id">Die ID des zu deaktivierenden Artikels.</param>
        /// <returns>Leitet zur Artikeldetailseite weiter.</returns>
        public ActionResult Deactivate(int id)
        {
            // Holt den ausgewählten Artikel und seine Bestandsinformationen aus der Datenbank
            var selectedItem = db.tblItems.Include(x => x.tblStocks).Where(x => x.Id == id).Single();
            var itemsInStock = selectedItem.tblStocks.Sum(x => x.Quantity);

            // Überprüft, ob der Artikel auf Lager ist
            if (itemsInStock == 0)
            {
                // Deaktiviert den Artikel, wenn er nicht auf Lager ist
                selectedItem.IsActive = "N";
                db.Entry(selectedItem).State = EntityState.Modified;
                db.SaveChanges();
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Artikel {0} deaktiviert.", selectedItem.Name) };
            }
            else
            {
                // Zeigt eine Fehlermeldung an, wenn der Artikel noch auf Lager ist
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Fehler!", Message = string.Format(" Es sind {0}  Artikel auf Lager. Der Artikel kann nicht gelöscht werden.", itemsInStock) };
            }

            return RedirectToAction("ItemDetails");
        }

        /// <summary>
        /// Aktiviert einen Artikel.
        /// </summary>
        /// <param name="id">Die ID des zu aktivierenden Artikels.</param>
        /// <returns>Leitet zur Artikeldetailseite weiter.</returns>
        public ActionResult Activate(int id)
        {
            // Holt den ausgewählten Artikel aus der Datenbank
            var selectedItem = db.tblItems.Include(x => x.tblStocks).Where(x => x.Id == id).Single();
            // Aktiviert den Artikel
            selectedItem.IsActive = "Y";
            db.Entry(selectedItem).State = EntityState.Modified;
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Artikel {0} aktiviert.", selectedItem.Name) };

            return RedirectToAction("ItemDetails");
        }
    }
}