using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.ViewModels.Manage
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل شما نامعتبر است.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور جدید")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور جدید")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
