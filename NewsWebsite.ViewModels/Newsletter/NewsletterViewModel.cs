using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.ViewModels.Newsletter
{
    public class NewsletterViewModel
    {
        [JsonProperty("Id"),Display(Name ="ایمیل")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است."),EmailAddress(ErrorMessage ="ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }

        [JsonProperty("ردیف")]
        public int Row { get; set; }

        [JsonProperty("تاریخ عضویت")]
        public string PersianRegisterDateTime { get; set; }

        [JsonIgnore]
        public DateTime? RegisterDateTime { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
    }
}
