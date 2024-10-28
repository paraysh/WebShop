using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Entity;

namespace WebShop.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        int _userRole;
        private WebShopEntities db = new WebShopEntities();
        public ActionResult Index()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}