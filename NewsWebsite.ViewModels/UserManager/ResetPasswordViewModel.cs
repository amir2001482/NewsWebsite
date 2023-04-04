using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.ViewModels.UserManager
{
    public class ResetPasswordViewModel
    {
        public int userId { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [Display(Name = "کلمه عبور جدید")]
        public string NewPassword { get; set; }
    }
}
