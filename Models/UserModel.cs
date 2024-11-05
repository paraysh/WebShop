using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.WebPages.Html;
using WebShop.Models.Entity;
using WebShop.Models.Enum;

namespace WebShop.Models
{
    public class UserModel
    {
        public UserModel()
        {
            //this.tblOrders = new HashSet<tblOrder>();
            //this.tblTeamBudgets = new HashSet<tblTeamBudget>();
            //this.tblTeamEmployees = new HashSet<tblTeamEmployee>();
            //this.tblTeamEmployees1 = new HashSet<tblTeamEmployee>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Benutzername ist erforderlich")]
        [Display(Name = "Benutzername")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Der Vorname ist erforderlich")]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        [Display(Name = "Nachname")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }


        [Display(Name = "Rolle")]
        public Nullable<int> UserRole { get; set; }
        [Display(Name = "Ist aktiv?")]
        public string IsActive { get; set; }

        [Display(Name = "Team Budget")]
        public decimal? TeamBudget { get; set; } = 0;
        [Display(Name = "Employee Budget")]
        public decimal? EmployeeBudget { get; set; } = 0;
        public decimal? AssignedTeamBudget { get; set; } = 0;
        public decimal? RemainingTeamBudget { get; set; } = 0;

        public List<SelectListItem> availableTeamLeaderLst { get; set; }
        public int TeamLeader { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDt { get; set; }

        [Display(Name = "Passwort")]
        [Required(ErrorMessage = "Passwort ist erforderlich")]
        public string Password { get; set; }

        [Display(Name = "Passwort wiederholen")]
        public string ConfirmPassword { get; set; }

        public UserRoleEnum UserRoleEnum { get; set; }
    }
}