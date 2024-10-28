using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.Models.Entity;

namespace WebShop.Controllers
{
    public class AccountController : Controller
    {
        public WebShopEntities db = new WebShopEntities();
      

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user =  db.tblUsers.Where(x => x.UserName == model.UserName && x.Password == model.Password).FirstOrDefault();

                    if (user != null)
                    {
                        if (user.IsActive != "Y")
                        {
                            TempData["ErrorMessage"] = "<h3>Account Suspended</h3>Your account is suspended. Please contact your company administrator.";
                            return View(model);
                        }
                       
                        //sign user in
                        SignIn(user);
                        ViewBag.CurrentUser = user;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                catch (Exception ex)
                {
                    //Log exception
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void SignIn(tblUser user)
        {
            var claims = new Claim[] {
                            new Claim(ClaimTypes.NameIdentifier, user.UserRole.ToString()),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, user.tblUserRolesMaster.UserRole)
                        };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(identity);
        }

        public ActionResult SignOut() 
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }    
    }
}