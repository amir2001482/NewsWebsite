using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.ViewModels.Tag
{
    public class TagViewModel
    {
        [JsonProperty("Id")]
        public string TagId { get; set; }

        [JsonProperty("ردیف")]
        public int Row { get; set; }

        [JsonProperty("برچسب"),Display(Name = "عنوان برچسب")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string TagName { get; set; }
    }
}
