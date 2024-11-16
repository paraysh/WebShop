using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    /// <summary>
    /// Repräsentiert das Modell für die Benutzeranmeldung im WebShop.
    /// Diese Klasse enthält die notwendigen Eigenschaften für die Anmeldung eines Benutzers.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Der Benutzername des Benutzers.
        /// </summary>
        [Required]
        [Display(Name = "Benutzername")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        /// <summary>
        /// Das Passwort des Benutzers.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        /// <summary>
        /// Gibt an, ob der Benutzer angemeldet bleiben möchte.
        /// </summary>
        [Display(Name = "Angemeldet bleiben?")]
        public bool RememberMe { get; set; }
    }
}