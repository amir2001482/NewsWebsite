using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewsWebsite.Entities
{
    public class Comment
    {
        public Comment()
        {
            comments = new List<Comment>();
        }
        [Key]
        public string CommentId { get; set; }
        public string NewsId { get; set; }
        [ForeignKey("comment")]
        public string ParentCommentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Desription { get; set; }
        public bool IsConfirm { get; set; }
        public DateTime? PostageDateTime { get; set; }
        public virtual News News { get; set; }
        public virtual Comment comment { get; set; }
        public virtual ICollection<Comment> comments { get; set; }
    }
}
