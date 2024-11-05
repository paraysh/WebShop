using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models.Enum
{
    public enum UserRoleEnum
    {
        [Display(Name = "Lagermitarbeiter")]
        WarehouseWorkers = 10,
        [Display(Name = "Stammdatenverwalter")]
        MasterDataManager = 20,
        [Display(Name = "Administrator")]
        Admins = 30,
        [Display(Name = "Teamleiter")]
        TeamLeaders = 40,
        [Display(Name = "Mitarbeiter")]
        Employee = 50
    }
}