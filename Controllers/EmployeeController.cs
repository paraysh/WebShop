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
    public class EmployeeController : Controller
    {
        int _userRole;
        private WebShopEntities db = new WebShopEntities();
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(tblUser user)
        {
            db.tblUsers.Add(user);
            db.SaveChanges();
            return RedirectToAction("EmployeeManagement");
        }

        //[Authorize(Roles = "20")]  //Master Data Manager
        public ActionResult EmployeeManagement()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;
            List<tblUser> lstUsers = new List<tblUser>();
            lstUsers = db.tblUsers.ToList();
            return View(lstUsers);
        }

        public ActionResult Edit(int id)
        {
            var employee = db.tblUsers.Where(x => x.Id == id).SingleOrDefault();
            return View(employee);
        }

        [HttpPost]
        public ActionResult Edit(tblUser user)
        {
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EmployeeManagement");
        }

        public ActionResult Delete(int id)
        {
            var user = db.tblUsers.Where(x => x.Id == id).SingleOrDefault();
            db.tblUsers.Remove(user);
            db.SaveChanges();
            return RedirectToAction("EmployeeManagement");
        }

        public ActionResult Deactivate(int id)
        {
            var user = db.tblUsers.Where(x => x.Id == id).SingleOrDefault();
            user.IsActive = "N";
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EmployeeManagement");
        }
    }
}