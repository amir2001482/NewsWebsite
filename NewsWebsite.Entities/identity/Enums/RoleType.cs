using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.Entities.identity.Enums
{
    public enum RoleType
    {
       [Display(Name = "مدیر سایت")]
        admin = 1,
       [Display(Name = "کاربر")]
        user = 5,
    }
}
