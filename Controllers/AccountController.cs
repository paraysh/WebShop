//Berarbeiter: Yusuf Can Sönmez
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebShop.Helper;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Models.Enum;

namespace WebShop.Controllers
{
    /// <summary>
    /// Diese Klasse verwaltet die Benutzerkonten im WebShop. Sie bietet Funktionen zum Anmelden, Abmelden und zur Authentifizierung von Benutzern.
    /// </summary>
    public class AccountController : BaseController
    {
        WebShopEntities db = new WebShopEntities();
        public AccountController(WebShopEntities _db) : base(_db) 
        {
            db = _db;
        }

        public AccountController()
        {
                
        }

        /// <summary>
        /// Holt den Authentifizierungsmanager für die aktuelle HTTP-Anfrage.
        /// </summary>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        /// <summary>
        /// Zeigt die Anmeldeseite an.
        /// </summary>
        /// <param name="returnUrl">Die URL, zu der nach erfolgreicher Anmeldung weitergeleitet werden soll.</param>
        /// <returns>Die Ansicht der Anmeldeseite.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View();
        }

        /// <summary>
        /// Verarbeitet die Anmeldeinformationen des Benutzers.
        /// </summary>
        /// <param name="model">Das Anmeldemodell mit Benutzername und Passwort.</param>
        /// <returns>Leitet zur Produktseite weiter, wenn die Anmeldung erfolgreich ist, andernfalls wird die Anmeldeseite erneut angezeigt.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = db.tblUsers.Where(x => x.UserName == model.UserName).FirstOrDefault();

                    if (user != null)
                    {
                        // Prüft das Passwort gegen den Hash
                        byte[] hashBytes = user.HashPassword;
                        PasswordHash hash = new PasswordHash(hashBytes);
                        if (!hash.Verify(model.Password))
                        {
                            ModelState.AddModelError("", "Benutzername oder Passwort ungültig.");
                            return View(model);
                        }

                        if (user.IsActive != "Y")
                        {
                            TempData["ErrorMessage"] = "<h3>Account Suspended</h3>Your account is suspended. Please contact your company administrator.";
                            return View(model);
                        }

                        
                        ViewBag.CurrentUser = user;
                        // Benutzer anmelden
                        SignIn(user);
                        return RedirectToAction("Index", "Product");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Benutzername oder Passwort ungültig.");
                    }
                }
                catch (Exception ex)
                {
                    // Ausnahme protokollieren
                }
            }
            // Wenn wir hierher gelangen, ist etwas fehlgeschlagen, das Formular erneut anzeigen
            return View(model);
        }

        /// <summary>
        /// Meldet den Benutzer an und erstellt die entsprechenden Ansprüche.
        /// </summary>
        /// <param name="user">Das Benutzerobjekt, das angemeldet werden soll.</param>
        public void SignIn(tblUser user)
        {
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, user.UserRole.ToString()),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, user.tblUserRolesMaster.UserRole),
                            new Claim("UserId", user.Id.ToString())
                            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(identity);
        }

        /// <summary>
        /// Meldet den Benutzer ab und löscht die Sitzung.
        /// </summary>
        /// <returns>Leitet zur Anmeldeseite weiter.</returns>
        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut();

            // Warenkorb aktualisieren
            Session["CartCounter"] = null;
            Session["CartItem"] = null;
            return RedirectToAction("Login");
        }
    }
}