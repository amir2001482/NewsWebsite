using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.ViewModels.Manage
{
    public class ForgetPasswordViewModel
    {
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده نامعتبر است.")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Email { get; set; }
    }
}
