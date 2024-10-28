using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShop.Models.Enum
{
    public enum UserRoleEnum
    {
        [Display(Name = "Warehouse Workers")]
        WarehouseWorkers = 10,
        [Display(Name = "Master Data Manager")]
        MasterDataManager = 20,
        [Display(Name = "Admins")]
        Admins = 30,
        [Display(Name = "Team Leaders")]
        TeamLeaders = 40,
        [Display(Name = "Employee")]
        Employee = 50
    }
}