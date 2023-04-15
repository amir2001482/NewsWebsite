using Microsoft.AspNetCore.Http;
using NewsWebsite.Common.Attributes;
using NewsWebsite.Entities;
using NewsWebsite.Entities.identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace NewsWebsite.ViewModels.News
{
    public class NewsViewModel
    {
        [JsonPropertyName("Id")]
        public string NewsId { get; set; }
        [JsonPropertyName("ردیف")]
        public int Row { get; set; }
        [JsonPropertyName("عنوان خبر"), Display(Name = "عنوان خبر")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Title { get; set; }
        [JsonPropertyName("ShortTitle")]
        public string ShortTitle { get; set; }
        [JsonIgnore]
        public bool FuturePublish { get; set; }
        [JsonIgnore]
        public DateTime? PublishDateTime { get; set; }
        [Display(Name = "تاریخ انتشار"), JsonPropertyName("تاریخ انتشار")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianPublishDate { get; set; }
        [Display(Name = "زمان انتشار"), JsonIgnore]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string PersianPublishTime { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonPropertyName("نویسنده")]
        public string AuthorName { get; set; }
        [JsonIgnore]
        public string ImageName { get; set; }
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        [JsonIgnore, Display(Name = "تصویر شاخص")]
        public string ImageFile { get; set; }
        [JsonIgnore]
        public bool IsPublish { get; set; }
        [Required(ErrorMessage = "انتخاب {0} الزامی است.")]
        [Display(Name = "نوع خبر"), JsonIgnore()]
        public bool? IsInternal { get; set; }
        [JsonPropertyName("تگ ها")]
        public string NameOfTags { get; set; }
        [JsonPropertyName("نوع خبر")]
        public string NewsType { get; set; }
        [JsonPropertyName("بازدید")]
        public int NumberOfVisit { get; set; }
        [JsonPropertyName("NumberOfLike")]
        public int NumberOfLike { get; set; }
        [JsonPropertyName("NumberOfDisLike")]
        public int NumberOfDisLike { get; set; }
        [JsonPropertyName("دسته")]
        public string NameOfCategories { get; set; }
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [Display(Name = "آدرس خبر"), JsonPropertyName("آدرس")]
        [UrlValidate("/", @"\", " ")]
        public string Url { get; set; }
        [JsonPropertyName("Status")]
        public string Status { get; set; }
        [JsonPropertyName("متن خبر")]
        public string Description { get; set; }
        [JsonIgnore]
        [Required(ErrorMessage = "انتخاب {0} الزامی است.") , Display(Name ="چکیده")]
        public string Abstract { get; set; }
        [JsonPropertyName("NumberOfComment")]
        public int NumberOfComment { get; set; }
        [JsonIgnore]
        public string[] CategoryIds { get; set; }
        [JsonIgnore]
        public string IdOfTags { get; set; }
        [JsonIgnore]
        public List<string> TagNamesList { get; set; }
        [JsonIgnore]
        public List<string> TagIdsList { get; set; }
        [JsonIgnore]
        public User AuthorInfo { get; set; }
        [JsonIgnore]
        public bool Bookmarked { get; set; }
        [JsonIgnore]
        public string SearchText { get; set; }
        [JsonIgnore]
        public NewsCategoriesViewModel NewsCategoriesViewModel { get; set; }
        [JsonIgnore]
        public virtual ICollection<NewsCategory> NewsCategories { get; set; }
        [JsonIgnore]
        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
