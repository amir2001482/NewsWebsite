using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.Entities.identity.Enums
{
    public enum GenderType
    {
        [Display(Name = "مرد")]
        male = 1,
        [Display(Name = "زن")]
        famale = 2
    }
}
