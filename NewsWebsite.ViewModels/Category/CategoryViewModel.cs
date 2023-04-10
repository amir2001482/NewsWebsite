
using NewsWebsite.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace NewsWebsite.ViewModels.Category
{
    public class CategoryViewModel
    {
        [JsonPropertyName("Id")]
        public string CategoryId { get; set; }

        [Display(Name ="عنوان دسته بندی"), JsonPropertyName("دسته")]
        [Required(ErrorMessage ="وارد نمودن {0} الزامی است.")]
        public string CategoryName { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [Display(Name ="دسته پدر"), JsonPropertyName("دسته پدر")]
        public string ParentCategoryName { get; set; }

        [JsonIgnore]
        public string ParentCategoryId { get; set; }


        [Display(Name = "آدرس"), JsonPropertyName("آدرس")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [UrlValidate("/" , @"\" , " ")]
        public string Url { get; set; }
    }
}
