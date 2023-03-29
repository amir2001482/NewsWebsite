using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.ViewModels.SiteSetting
{
    public class SettingsViewModel
    {

        [Display(Name="عنوان سایت")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        public string Title { get; set; }

        [Display(Name = "معرفی سایت")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Description { get; set; }

        [Display(Name = "متاتگ توضیحات")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string MetaDescriptionTag { get; set; }

        public string LogoName { get; set; }

        [Display(Name = "لوگو")]
        public IFormFile Logo{ get; set; }

        public string FaviconName { get; set; }

        [Display(Name = "فاویکن")]
        public IFormFile Favicon { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string EmailUsername { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage ="ایمیل وارد شده نامعتبر است.")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string SenderEmail { get; set; }

        [Display(Name = "پسورد")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string EmailPassword { get; set; }

        [Display(Name = "پرت")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public int EmailPort { get; set; }

        [Display(Name = "آدرس هاست")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string EmailHost { get; set; }
    }
}
