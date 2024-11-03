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
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "User Role")]
        public Nullable<int> UserRole { get; set; }
        [Display(Name = "Is Active?")]
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
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public UserRoleEnum UserRoleEnum { get; set; }
    }
}