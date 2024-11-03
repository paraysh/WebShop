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
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            UserModel usr = new UserModel();
            //create available team leaders select list
            var admins = db.tblUsers.Where(x => x.UserRole.Value == (int)UserRoleEnum.TeamLeaders).Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.Id.ToString()
            }).ToList();
            usr.availableTeamLeaderLst = admins;
            return View(usr);
        }

        [HttpPost]
        public ActionResult Add(UserModel user)
        {
            tblUser newUserObj = new tblUser();
            newUserObj.UserName = user.UserName;
            newUserObj.FirstName = user.FirstName;
            newUserObj.LastName = user.LastName;
            newUserObj.Email = user.Email;
            newUserObj.UserRole = user.UserRole;
            newUserObj.Password = user.UserName; // setting password same as userName
            newUserObj.IsActive = user.IsActive;

            // check if userrole is "team leader"
            if (user.UserRole.Value == (int)UserRoleEnum.TeamLeaders && user.TeamBudget.HasValue)
            {
                tblTeamBudget _tblbudgetObj = new tblTeamBudget();
                _tblbudgetObj.TeamLeaderId = user.Id;
                _tblbudgetObj.TeamBudget = user.TeamBudget;
                newUserObj.tblTeamBudgets.Add(_tblbudgetObj);

            }

            // check if userrole is "Employee"
            if (user.UserRole.Value == (int)UserRoleEnum.Employee && user.EmployeeBudget.HasValue)
            {
                tblTeamEmployee _tblTeamObj = new tblTeamEmployee();
                _tblTeamObj.TeamEmployeeId = user.Id;
                _tblTeamObj.TeamLeaderId = user.TeamLeader;
                _tblTeamObj.TeamEmployeeBudget = user.EmployeeBudget;
                newUserObj.tblTeamEmployees.Add(_tblTeamObj);
            }

            db.tblUsers.Add(newUserObj);
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("User {0} Added.", user.UserName) };
            return RedirectToAction("EmployeeManagement");
        }

        //[Authorize(Roles = "20")]  //Master Data Manager
        public ActionResult EmployeeManagement()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            List<tblUser> lstUsers = new List<tblUser>();
            lstUsers = db.tblUsers.Include(x => x.tblUserRolesMaster).ToList();
            return View(lstUsers);
        }

        public ActionResult Edit(int id)
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            var employee = db.tblUsers.Where(x => x.Id == id).Include(x => x.tblUserRolesMaster).Include(x => x.tblTeamBudgets).Include(x => x.tblTeamEmployees).SingleOrDefault();
            UserModel usr = new UserModel();
            usr.Id = employee.Id;
            usr.UserName = employee.UserName;
            usr.IsActive = employee.IsActive;
            usr.FirstName = employee.FirstName;
            usr.LastName = employee.LastName;
            usr.Email = employee.Email;
            usr.UserRole = employee.UserRole;
            usr.TeamBudget = employee.tblTeamBudgets.SingleOrDefault() == null ? 0 : employee.tblTeamBudgets.SingleOrDefault().TeamBudget;
            usr.Password = employee.Password;
            usr.TeamLeader = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).FirstOrDefault() == null ? 0 : employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).FirstOrDefault().TeamLeaderId;
            usr.EmployeeBudget = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).SingleOrDefault() == null ? 0 : employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).SingleOrDefault().TeamEmployeeBudget;

            //create available team leaders select list
            var admins = db.tblUsers.Where(x => x.UserRole.Value == (int)UserRoleEnum.TeamLeaders).Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.Id.ToString()
            }).ToList();

            usr.availableTeamLeaderLst = admins;

            return View(usr);
        }

        [HttpPost]
        public ActionResult Edit(UserModel user)
        {
            var employee = db.tblUsers.Where(x => x.Id == user.Id).Include(x => x.tblUserRolesMaster).Include(x => x.tblTeamBudgets).Include(x => x.tblTeamEmployees).SingleOrDefault();
            employee.UserName = user.UserName;
            employee.FirstName = user.FirstName;
            employee.LastName = user.LastName;
            employee.Email = user.Email;
            employee.UserRole = user.UserRole;
            employee.Password = user.UserName; // setting password same as userName
            employee.IsActive = user.IsActive;

            // check if userrole is "team leader"
            if (user.UserRole.Value == (int)UserRoleEnum.TeamLeaders && user.TeamBudget.HasValue)
            {
                if (employee.tblTeamBudgets.Count > 0)
                {
                    var teamBudgetRow = employee.tblTeamBudgets.SingleOrDefault();
                    teamBudgetRow.TeamBudget = user.TeamBudget;
                }
                else
                {
                    tblTeamBudget _tblbudgetObj = new tblTeamBudget();
                    _tblbudgetObj.TeamLeaderId = user.Id;
                    _tblbudgetObj.TeamBudget = user.TeamBudget;
                    employee.tblTeamBudgets.Add(_tblbudgetObj);
                }
            }

            // check if userrole is "Employee"
            if (user.UserRole.Value == (int)UserRoleEnum.Employee && user.EmployeeBudget.HasValue)
            {
                if (employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == user.Id).Count() > 0)
                {
                    var teamEmployeeRow = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == user.Id).SingleOrDefault();
                    teamEmployeeRow.TeamLeaderId = user.TeamLeader;
                    teamEmployeeRow.TeamEmployeeBudget = user.EmployeeBudget;
                }
                else
                {
                    tblTeamEmployee _tblTeamObj = new tblTeamEmployee();
                    _tblTeamObj.TeamEmployeeId = user.Id;
                    _tblTeamObj.TeamLeaderId = user.TeamLeader;
                    _tblTeamObj.TeamEmployeeBudget = user.EmployeeBudget;
                    employee.tblTeamEmployees.Add(_tblTeamObj);
                }
            }

            db.Entry(employee).State = EntityState.Modified;
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("User {0} Updated.", user.UserName) };
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

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Success!", Message = string.Format("User {0} Deactivate.", user.UserName) };
            return RedirectToAction("EmployeeManagement");
        }
    }
}