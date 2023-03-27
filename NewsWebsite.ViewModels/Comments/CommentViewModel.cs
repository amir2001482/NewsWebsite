using System;
using System.Collections.Generic;
using NewsWebsite.Entities;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Comments
{
    public class CommentViewModel
    {
        public CommentViewModel()
        {

        }

        public CommentViewModel(string parentCommentId, string newsId)
        {
            ParentCommentId = parentCommentId;
            NewsId = newsId;
        }

        [JsonProperty("Id")]
        public string CommentId { get; set; }

        [JsonProperty("ردیف")]
        public int Row { get; set; }

        [JsonProperty("نام"),Display(Name="نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Name { get; set; }


        [JsonProperty("ایمیل"),Display(Name = "ایمیل")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [EmailAddress(ErrorMessage ="ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [JsonProperty("دیدگاه") , Display(Name = "دیدگاه")]
        public string Desription { get; set; }

        [JsonIgnore]
        public string NewsId { get; set; }

        [JsonProperty("IsConfirm")]
        public bool IsConfirm { get; set; }

        [JsonIgnore]
        public DateTime? PostageDateTime { get; set; }

        [JsonProperty("تاریخ ارسال")]
        public string PersianPostageDateTime { get; set; }

        [JsonIgnore]
        public string ParentCommentId { get; set; }

        [JsonIgnore]
        public virtual List<CommentViewModel> comments { get; set; }
    }
}
