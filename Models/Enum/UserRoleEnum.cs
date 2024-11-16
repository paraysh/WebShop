using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models.Enum
{
    /// <summary>
    /// Repräsentiert die verschiedenen Benutzerrollen im WebShop.
    /// Diese Enumeration wird verwendet, um die Rolle eines Benutzers zu definieren.
    /// </summary>
    public enum UserRoleEnum
    {
        /// <summary>
        /// Lagermitarbeiter-Rolle.
        /// </summary>
        [Display(Name = "Lagermitarbeiter")]
        WarehouseWorkers = 10,

        /// <summary>
        /// Stammdatenverwalter-Rolle.
        /// </summary>
        [Display(Name = "Stammdatenverwalter")]
        MasterDataManager = 20,

        /// <summary>
        /// Administrator-Rolle.
        /// </summary>
        [Display(Name = "Administrator")]
        Admins = 30,

        /// <summary>
        /// Teamleiter-Rolle.
        /// </summary>
        [Display(Name = "Teamleiter")]
        TeamLeaders = 40,

        /// <summary>
        /// Mitarbeiter-Rolle.
        /// </summary>
        [Display(Name = "Mitarbeiter")]
        Employee = 50
    }
}