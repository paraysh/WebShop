using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Entity;
using WebShop.Models.Enum;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert ein Benutzermodell im WebShop.
    /// Diese Klasse enthält die grundlegenden Eigenschaften eines Benutzers, wie Benutzername, Vorname, Nachname, Email, Rolle und Budgets.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="UserModel"/> Klasse.
        /// </summary>
        public UserModel()
        {
            //this.tblOrders = new HashSet<tblOrder>();
            //this.tblTeamBudgets = new HashSet<tblTeamBudget>();
            //this.tblTeamEmployees = new HashSet<tblTeamEmployee>();
            //this.tblTeamEmployees1 = new HashSet<tblTeamEmployee>();
            ItemsOrdered = new List<ItemModelEmployee>();
        }

        /// <summary>
        /// Die eindeutige Identifikationsnummer des Benutzers.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Der Benutzername des Benutzers.
        /// </summary>
        [Required(ErrorMessage = "Benutzername ist erforderlich")]
        [Display(Name = "Benutzername")]
        public string UserName { get; set; }

        /// <summary>
        /// Der Vorname des Benutzers.
        /// </summary>
        [Required(ErrorMessage = "Vorname ist erforderlich")]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Der Nachname des Benutzers.
        /// </summary>
        [Required(ErrorMessage = "Nachname ist erforderlich")]
        [Display(Name = "Nachname")]
        public string LastName { get; set; }

        /// <summary>
        /// Die Email-Adresse des Benutzers.
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Die Rolle des Benutzers.
        /// </summary>
        [Display(Name = "Rolle")]
        public Nullable<int> UserRole { get; set; }

        /// <summary>
        /// Gibt an, ob der Benutzer aktiv ist.
        /// </summary>
        [Display(Name = "Ist aktiv?")]
        public string IsActive { get; set; }

        /// <summary>
        /// Das Team-Budget des Benutzers.
        /// </summary>
        [Display(Name = "Team Budget")]
        public string TeamBudget { get; set; } = "0,00";

        /// <summary>
        /// Das Mitarbeiter-Budget des Benutzers.
        /// </summary>
        [Display(Name = "Employee Budget")]
        public string EmployeeBudget { get; set; } = "0,00";

        /// <summary>
        /// Das zugewiesene Team-Budget des Benutzers.
        /// </summary>
        public string AssignedTeamBudget { get; set; } = "0,00";

        /// <summary>
        /// Das verbleibende Team-Budget des Benutzers.
        /// </summary>
        public string RemainingTeamBudget { get; set; } = "0,00";

        /// <summary>
        /// Eine Liste der verfügbaren Teamleiter.
        /// </summary>
        public List<SelectListItem> availableTeamLeaderLst { get; set; }

        /// <summary>
        /// Der Teamleiter des Benutzers.
        /// </summary>
        public int TeamLeader { get; set; }

        /// <summary>
        /// Das Passwort des Benutzers.
        /// </summary>
        [Display(Name = "Passwort")]
        [Required(ErrorMessage = "Passwort ist erforderlich")]
        public string Password { get; set; }

        /// <summary>
        /// Das Bestätigungspasswort des Benutzers.
        /// </summary>
        [Display(Name = "Passwort wiederholen")]
        [Required(ErrorMessage = "Passwort ist erforderlich")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Die Benutzerrolle als Enumeration.
        /// </summary>
        public UserRoleEnum UserRoleEnum { get; set; }

        public byte[] HashPassword { get; set; }

        public List<ItemModelEmployee> ItemsOrdered { get; set; }
    }
}