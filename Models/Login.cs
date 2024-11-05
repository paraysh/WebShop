using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Benutzername")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string Password { get; set; }

        [Display(Name = "Angemeldet bleiben?")]
        public bool RememberMe { get; set; }
    }
}