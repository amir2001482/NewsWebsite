using Microsoft.AspNetCore.Http;
using NewsWebsite.Entities.identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NewsWebsite.Entities.identity.Enums;

namespace NewsWebsite.ViewModels.UserManager
{
    public class UsersViewModel
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [Display(Name = "تصویر پروفایل"), JsonPropertyName("تصویر")]
        public string Image { get; set; }

        [JsonIgnore,Display(Name ="تصویر پروفایل")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public IFormFile ImageFile { get; set; }


        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        [Display(Name="نام کاربری"), JsonPropertyName("نام کاربری")]
        public string UserName { get; set; }
        
        [Display(Name ="ایمیل"), JsonPropertyName("ایمیل")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [EmailAddress(ErrorMessage ="ایمیل وارد شده صحیح نمی باشد.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید دارای حداقل {2} کاراکتر و حداکثر دارای {1} کاراکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password), Display(Name = "کلمه عبور"),JsonIgnore]
        public string Password { get; set; }

        [DataType(DataType.Password), Display(Name = "تکرار کلمه عبور"),JsonIgnore]
        [Compare("Password", ErrorMessage = "کلمه عبور وارد شده با تکرار کلمه عبور مطابقت ندارد.")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "شماره موبایل"), JsonPropertyName("شماره تماس")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نام"), JsonPropertyName("نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی"), JsonPropertyName("نام خانوادگی")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string LastName { get; set; }

        [Display(Name = "تاریخ تولد"),JsonIgnore()]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "تاریخ تولد"), JsonPropertyName("تاریخ تولد")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianBirthDate { get; set; }
        
        [Display(Name = "تاریخ عضویت"),JsonIgnore]
        public DateTime? RegisterDateTime { get; set; }

        [Display(Name = "تاریخ عضویت"), JsonPropertyName("تاریخ عضویت")]
        public string PersianRegisterDateTime { get; set; }

        [Display(Name = "فعال / غیرفعال"), JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }

        [Display(Name = "جنسیت"),JsonIgnore]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public GenderType? Gender { get; set; }

        [JsonPropertyName("جنسیت")]
        public string GenderName { get; set; }
        [Display(Name = "معرفی"), JsonPropertyName("معرفی")]
        public string Bio { get; set; }

        [JsonIgnore]
        public ICollection<UserRole> Roles { get; set; }

        [JsonIgnore,Display(Name ="نقش")]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        public int? RoleId { get; set; }

        [JsonPropertyName("نقش")]
        public string RoleName { get; set; }
        [JsonIgnore]
        public RoleType RoleEnum { get; set; }

        [JsonIgnore]
        public bool PhoneNumberConfirmed { get; set; }

        [JsonIgnore]
        public bool TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public bool LockoutEnabled { get; set; }

        [JsonIgnore]
        public bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public int AccessFailedCount { get; set; }

        [JsonIgnore]
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
