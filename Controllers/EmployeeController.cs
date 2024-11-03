using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
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
        ClaimsPrincipal prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
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
            usr.UserRoleEnum = UserRoleEnum.MasterDataManager; // default value for dropdown

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
            if (user.Password != user.ConfirmPassword)
            {
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = string.Format("Unable to Add. Password mismatched.") };
                return RedirectToAction("EmployeeManagement");
            }
            tblUser newUserObj = new tblUser();
            newUserObj.UserName = user.UserName;
            newUserObj.FirstName = user.FirstName;
            newUserObj.LastName = user.LastName;
            newUserObj.Email = user.Email;
            newUserObj.UserRole = (int)user.UserRoleEnum;
            newUserObj.Password = user.Password; // setting password same as userName
            newUserObj.IsActive = user.IsActive;

            // check if userrole is "team leader"
            if (user.UserRoleEnum == UserRoleEnum.TeamLeaders)
            {
                tblTeamBudget _tblbudgetObj = new tblTeamBudget();
                _tblbudgetObj.TeamLeaderId = user.Id;
                _tblbudgetObj.TeamBudget = user.TeamBudget;
                newUserObj.tblTeamBudgets.Add(_tblbudgetObj);

            }

            // check if userrole is "Employee"
            if (user.UserRoleEnum == UserRoleEnum.Employee)
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
            int userID = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            ViewBag.UserRole = _userRole;
            ViewBag.UserId = userID;

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
            usr.UserRoleEnum = (UserRoleEnum)employee.UserRole;
            usr.TeamBudget = employee.tblTeamBudgets.SingleOrDefault() == null ? 0 : employee.tblTeamBudgets.SingleOrDefault().TeamBudget;
            usr.Password = employee.Password;
            usr.ConfirmPassword = employee.Password;
            usr.TeamLeader = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).FirstOrDefault() == null ? 0 : employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).FirstOrDefault().TeamLeaderId;
            usr.EmployeeBudget = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).SingleOrDefault() == null ? 0 : employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).SingleOrDefault().TeamEmployeeBudget;

            if (usr.UserRole == (int)UserRoleEnum.Employee)
            {
                usr.AssignedTeamBudget = db.tblTeamBudgets.Where(x => x.TeamLeaderId == usr.TeamLeader).Single().TeamBudget;
                var teamEmployeesTotalBudget = db.tblTeamEmployees.Where(x => x.TeamLeaderId == usr.TeamLeader).Sum(x => x.TeamEmployeeBudget);
                usr.RemainingTeamBudget = usr.AssignedTeamBudget - (teamEmployeesTotalBudget == null ? 0 : teamEmployeesTotalBudget);
            }
            
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
            employee.UserRole = (int)user.UserRoleEnum;
            employee.Password = user.Password; 
            employee.IsActive = user.IsActive;

            // check if userrole is "team leader"
            if (user.UserRoleEnum == UserRoleEnum.TeamLeaders && user.TeamBudget.HasValue)
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
            if (user.UserRoleEnum == UserRoleEnum.Employee && user.EmployeeBudget.HasValue)
            {
                if (user.EmployeeBudget > user.RemainingTeamBudget)
                {
                    TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = string.Format("Unable to Update. Remaining Team budget is less than Employee Budget.") };
                    return RedirectToAction("EmployeeManagement");
                }

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

            if (user.Password != user.ConfirmPassword)
            {
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Error!", Message = string.Format("Unable to Update. Password mismatched.") };
                return RedirectToAction("EmployeeManagement");
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