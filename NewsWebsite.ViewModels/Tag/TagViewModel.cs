
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace NewsWebsite.ViewModels.Tag
{
    public class TagViewModel
    {
        [JsonPropertyName("Id")]
        public string TagId { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [JsonPropertyName("برچسب"),Display(Name = "عنوان برچسب")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string TagName { get; set; }
    }
}
