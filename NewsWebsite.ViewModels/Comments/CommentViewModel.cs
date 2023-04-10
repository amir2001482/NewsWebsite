using System;
using System.Collections.Generic;
using NewsWebsite.Entities;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonPropertyName("Id")]
        public string CommentId { get; set; }

        [JsonPropertyName("ردیف")]
        public int Row { get; set; }

        [JsonPropertyName("نام"),Display(Name="نام")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        public string Name { get; set; }


        [JsonPropertyName("ایمیل"),Display(Name = "ایمیل")]
        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [EmailAddress(ErrorMessage ="ایمیل وارد شده معتبر نمی باشد.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
        [JsonPropertyName("دیدگاه") , Display(Name = "دیدگاه")]
        public string Desription { get; set; }

        [JsonIgnore]
        public string NewsId { get; set; }

        [JsonPropertyName("IsConfirm")]
        public bool? IsConfirm { get; set; }

        [JsonIgnore]
        public DateTime? PostageDateTime { get; set; }

        [JsonPropertyName("تاریخ ارسال")]
        public string PersianPostageDateTime { get; set; }

        [JsonIgnore]
        public string ParentCommentId { get; set; }

        [JsonIgnore]
        public virtual List<CommentViewModel> comments { get; set; }
    }
}
