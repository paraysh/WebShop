//Bearbeiter: Abbas Dayeh(Mitarbeiteranlegen/ deaktivieren Funktion)
//            Alper Daglioglu(Mitarbeiteranlegen/ deaktivieren Funktion)
//            Bekir Kurtuldu(Mitarbeiterverwaltung)   
//            Yusuf Can Sönmez(Mitarbeiterverwaltung Budget Bugfix)
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebShop.Helper;
using WebShop.Models;
using WebShop.Models.Entity;
using WebShop.Models.Enum;

namespace WebShop.Controllers
{
    /// <summary>
    /// Der EmployeeController verwaltet die Mitarbeiter im WebShop.
    /// Er bietet Funktionen zum Hinzufügen, Bearbeiten, Löschen und Deaktivieren von Mitarbeitern sowie zur Verwaltung der Mitarbeiterübersicht.
    /// </summary>
    [Authorize]
    public class EmployeeController : Controller
    {
        int _userRole;
        ClaimsPrincipal prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
        private WebShopEntities db = new WebShopEntities();

        /// <summary>
        /// Zeigt die Startseite für Mitarbeiter an.
        /// </summary>
        /// <returns>Die Index-Ansicht.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Zeigt die Seite zum Hinzufügen eines neuen Mitarbeiters an.
        /// </summary>
        /// <returns>Die Add-Ansicht mit einem neuen UserModel.</returns>
        public ActionResult Add()
        {
            _userRole = User.Identity.GetUserId<int>();
            ViewBag.UserRole = _userRole;

            UserModel usr = new UserModel();
            // Erstellen der verfügbaren Teamleiter-Auswahlliste
            usr.UserRoleEnum = UserRoleEnum.MasterDataManager; // Standardwert für Dropdown

            var admins = db.tblUsers.Where(x => x.UserRole.Value == (int)UserRoleEnum.TeamLeaders).Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.Id.ToString()
            }).ToList();
            usr.availableTeamLeaderLst = admins;
            return View(usr);
        }

        /// <summary>
        /// Fügt einen neuen Mitarbeiter hinzu.
        /// </summary>
        /// <param name="user">Das UserModel des hinzuzufügenden Mitarbeiters.</param>
        /// <returns>Leitet zur Mitarbeiterverwaltungsseite weiter.</returns>
        [HttpPost]
        public ActionResult Add(UserModel user)
        {
            if (user.Password != user.ConfirmPassword)
            {
                TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Fehler!", Message = string.Format("Aktualisierung nicht möglich. Das Passwort stimmt nicht überein.") };
                return RedirectToAction("EmployeeManagement");
            }
            tblUser newUserObj = new tblUser();
            newUserObj.UserName = user.UserName;
            newUserObj.FirstName = user.FirstName;
            newUserObj.LastName = user.LastName;
            newUserObj.Email = user.Email;
            newUserObj.UserRole = (int)user.UserRoleEnum;
            // Passwort-Hash speichern
            PasswordHash hash = new PasswordHash(user.Password);
            byte[] hashBytes = hash.ToArray();
            newUserObj.HashPassword = hashBytes;
            newUserObj.IsActive = user.IsActive;

            // Überprüfen, ob die Benutzerrolle "Teamleiter" ist
            if (user.UserRoleEnum == UserRoleEnum.TeamLeaders)
            {
                tblTeamBudget _tblbudgetObj = new tblTeamBudget();
                _tblbudgetObj.TeamLeaderId = user.Id;
                _tblbudgetObj.TeamBudget = user.TeamBudget;
                newUserObj.tblTeamBudgets.Add(_tblbudgetObj);
            }

            // Überprüfen, ob die Benutzerrolle "Mitarbeiter" ist
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
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("{0} hinzugefügt.", user.UserName) };
            return RedirectToAction("EmployeeManagement");
        }

        /// <summary>
        /// Zeigt die Mitarbeiterverwaltungsseite an.
        /// </summary>
        /// <returns>Die EmployeeManagement-Ansicht mit einer Liste von Mitarbeitern.</returns>
        public ActionResult EmployeeManagement()
        {
            _userRole = User.Identity.GetUserId<int>();
            int userID = Convert.ToInt32(prinicpal.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).SingleOrDefault());
            ViewBag.UserRole = _userRole;
            ViewBag.UserId = userID;

            List<tblUser> lstUsers = new List<tblUser>();

            if (_userRole == (int)UserRoleEnum.TeamLeaders)
            {
                var selfRow = db.tblUsers.Include(x => x.tblUserRolesMaster).Where(x => x.Id == userID).First();
                lstUsers = db.tblTeamEmployees.Where(x => x.TeamLeaderId == userID).Select(s => s.tblUser).ToList();
                lstUsers.Add(selfRow);
            }
            else
            {
                lstUsers = db.tblUsers.Include(x => x.tblUserRolesMaster).ToList();
            }
            return View(lstUsers);
        }

        /// <summary>
        /// Zeigt die Seite zum Bearbeiten eines Mitarbeiters an.
        /// </summary>
        /// <param name="id">Die ID des zu bearbeitenden Mitarbeiters.</param>
        /// <returns>Die Edit-Ansicht mit dem UserModel des Mitarbeiters.</returns>
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
            usr.TeamBudget = employee.tblTeamBudgets.SingleOrDefault() == null ? "0,00" : employee.tblTeamBudgets.SingleOrDefault().TeamBudget;
            usr.TeamLeader = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).FirstOrDefault() == null ? 0 : employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).FirstOrDefault().TeamLeaderId;
            usr.EmployeeBudget = employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).SingleOrDefault() == null ? "0,00" : employee.tblTeamEmployees.Where(x => x.TeamEmployeeId == id).SingleOrDefault().TeamEmployeeBudget;

            if (usr.UserRole == (int)UserRoleEnum.Employee)
            {
                usr.AssignedTeamBudget = db.tblTeamBudgets.Where(x => x.TeamLeaderId == usr.TeamLeader).Single().TeamBudget;
                var lstteamEmployeesTotalBudget = db.tblTeamEmployees.Where(x => x.TeamLeaderId == usr.TeamLeader).Select(x => x.TeamEmployeeBudget).ToList();

                decimal teamEmployeesTotalBudget = 0;
                foreach (var budget in lstteamEmployeesTotalBudget)
                    teamEmployeesTotalBudget += decimal.Parse(budget, new NumberFormatInfo() { NumberDecimalSeparator = "," });
                usr.RemainingTeamBudget = Convert.ToString(decimal.Parse(usr.AssignedTeamBudget, new NumberFormatInfo() { NumberDecimalSeparator = "," }) - teamEmployeesTotalBudget);
                usr.RemainingTeamBudget = usr.RemainingTeamBudget.Replace('.', ',');
            }

            // Erstellen der verfügbaren Teamleiter-Auswahlliste
            var admins = db.tblUsers.Where(x => x.UserRole.Value == (int)UserRoleEnum.TeamLeaders).Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.Id.ToString()
            }).ToList();
            usr.availableTeamLeaderLst = admins;

            // Mitarbeiter-Artikel
            List<ItemModelEmployee> itemsOrders = db.tblOrderDetails
                            .Include(x => x.tblOrder)
                            .Include(x => x.tblItem)
                            .Where(x => x.tblOrder.OrderedBy == id && x.tblOrder.OrderApproved == "Y")
                            .SelectMany(x => new List<ItemModelEmployee> {
                                    new ItemModelEmployee {
                                        ItemName = x.tblItem.Name,
                                        ImageName = x.tblItem.ImageName,
                                        LendingPeriodMonths = (int)x.LendingPeriodMonths,
                                        LendingStartDt = (DateTime)x.LendingStartDt,
                                        LendingEndDt = (DateTime)x.LendingEndDt,
                                    }
                            }).ToList();
            usr.ItemsOrdered = itemsOrders;

            return View(usr);
        }

        /// <summary>
        /// Aktualisiert die Informationen eines Mitarbeiters.
        /// </summary>
        /// <param name="user">Das aktualisierte UserModel des Mitarbeiters.</param>
        /// <returns>Leitet zur Mitarbeiterverwaltungsseite weiter.</returns>
        [HttpPost]
        public ActionResult Edit(UserModel user)
        {
            var employee = db.tblUsers.Where(x => x.Id == user.Id).Include(x => x.tblUserRolesMaster).Include(x => x.tblTeamBudgets).Include(x => x.tblTeamEmployees).SingleOrDefault();
            employee.UserName = user.UserName;
            employee.FirstName = user.FirstName;
            employee.LastName = user.LastName;
            employee.Email = user.Email;
            employee.UserRole = (int)user.UserRoleEnum;
            employee.IsActive = user.IsActive;

            // Überprüfen, ob die Benutzerrolle "Teamleiter" ist
            if (user.UserRoleEnum == UserRoleEnum.TeamLeaders && Convert.ToDecimal(user.TeamBudget) > 0)
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

            // Überprüfen, ob die Benutzerrolle "Mitarbeiter" ist
            if (user.UserRoleEnum == UserRoleEnum.Employee && Convert.ToDecimal(user.EmployeeBudget) > 0)
            {
                if (decimal.Parse(user.EmployeeBudget, new NumberFormatInfo() { NumberDecimalSeparator = "," }) > decimal.Parse(user.RemainingTeamBudget, new NumberFormatInfo() { NumberDecimalSeparator = "," }))
                {
                    TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-danger", Title = "Fehler!", Message = string.Format("Update nicht möglich. Das übrige Teambudget ist geringer als das Mitarbeiterbudget.") };
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

            db.Entry(employee).State = EntityState.Modified;
            db.SaveChanges();
            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("Benutzer {0} aktualisiert.", user.UserName) };
            return RedirectToAction("EmployeeManagement");
        }

        /// <summary>
        /// Löscht einen Mitarbeiter.
        /// </summary>
        /// <param name="id">Die ID des zu löschenden Mitarbeiters.</param>
        /// <returns>Leitet zur Mitarbeiterverwaltungsseite weiter.</returns>
        public ActionResult Delete(int id)
        {
            var user = db.tblUsers.Where(x => x.Id == id).SingleOrDefault();
            db.tblUsers.Remove(user);
            db.SaveChanges();
            return RedirectToAction("EmployeeManagement");
        }

        /// <summary>
        /// Deaktiviert einen Mitarbeiter.
        /// </summary>
        /// <param name="id">Die ID des zu deaktivierenden Mitarbeiters.</param>
        /// <returns>Leitet zur Mitarbeiterverwaltungsseite weiter.</returns>
        public ActionResult Deactivate(int id)
        {
            var user = db.tblUsers.Where(x => x.Id == id).SingleOrDefault();
            user.IsActive = "N";
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["UserMessage"] = new MessageVM() { CssClassName = "alert-success", Title = "Erledigt!", Message = string.Format("{0} deaktiviert.", user.UserName) };
            return RedirectToAction("EmployeeManagement");
        }
    }
}