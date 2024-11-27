//Bearbeiter: Abbas Dayeh(Navigationselement)
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Entity;

namespace WebShop.Controllers
{
    /// <summary>
    /// Diese Klasse verwaltet die Hauptseite des WebShops.
    /// Sie ermöglicht das Anzeigen der Startseite.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        // Speichert die Rolle des Benutzers
        int _userRole;
        // Datenbankkontext für den Zugriff auf die Datenbank
        private WebShopEntities db = new WebShopEntities();

        /// <summary>
        /// Zeigt die Startseite an.
        /// </summary>
        /// <returns>Eine Ansicht der Startseite.</returns>
        public ActionResult Index()
        {
            // Holt die Benutzerrolle des aktuellen Benutzers
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            return View();
        }
    }
}