
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NewsWebsite.ViewModels.Newsletter
{
    public class NewsletterViewModel
    {
        [JsonPropertyName("Id"),Display(Name ="ایمیل")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است."),EmailAddress(ErrorMessage ="ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [JsonPropertyName("تاریخ عضویت")]
        public string PersianRegisterDateTime { get; set; }

        [JsonIgnore]
        public DateTime? RegisterDateTime { get; set; }

        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; }
    }
}
