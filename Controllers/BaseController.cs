//Bearbeiter: Yusuf Can Sönmez 
using System.Security.Claims;
using System.Web.Mvc;
using WebShop.Models.Entity;

namespace WebShop.Controllers
{
    /// <summary>
    /// Die BaseController-Klasse dient als Basisklasse für alle Controller im WebShop.
    /// Sie stellt grundlegende Funktionalitäten und Eigenschaften zur Verfügung, die von anderen Controllern geerbt werden können.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Die Datenbankinstanz, die für Datenbankoperationen verwendet wird.
        /// </summary>
        public readonly WebShopEntities _db;

        /// <summary>
        /// Die ClaimsPrincipal-Instanz, die die Ansprüche des aktuellen Benutzers enthält.
        /// </summary>
        public readonly ClaimsPrincipal _claimsPrincipal;

        /// <summary>
        /// Initialisiert eine neue Instanz der BaseController-Klasse mit der angegebenen Datenbankinstanz.
        /// </summary>
        /// <param name="db">Die Datenbankinstanz, die für Datenbankoperationen verwendet wird.</param>
        public BaseController(WebShopEntities db)
        {
            _db = db;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der BaseController-Klasse mit der angegebenen Datenbankinstanz und ClaimsPrincipal-Instanz.
        /// </summary>
        /// <param name="db">Die Datenbankinstanz, die für Datenbankoperationen verwendet wird.</param>
        /// <param name="claimsPrincipal">Die ClaimsPrincipal-Instanz, die die Ansprüche des aktuellen Benutzers enthält.</param>
        public BaseController(WebShopEntities db, ClaimsPrincipal claimsPrincipal)
        {
            _db = db;
            _claimsPrincipal = claimsPrincipal;
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der BaseController-Klasse mit einer neuen Datenbankinstanz.
        /// </summary>
        public BaseController()
        {
            _db = new WebShopEntities();
        }
    }
}