using System.Security.Claims;
using System.Web.Mvc;
using WebShop.Models.Entity;

namespace WebShop.Controllers
{
    public class BaseController : Controller
    {
        public readonly WebShopEntities _db;
        public readonly ClaimsPrincipal _claimsPrincipal;

        public BaseController(WebShopEntities db)
        {
            _db = db;
        }

        public BaseController(WebShopEntities db, ClaimsPrincipal claimsPrincipal)
        {
            _db = db;
            _claimsPrincipal = claimsPrincipal;
        }

        public BaseController()
        {
           _db = new WebShopEntities();
        }
    }
}