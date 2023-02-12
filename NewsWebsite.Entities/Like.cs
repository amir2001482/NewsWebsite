using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Entities
{
    public class Like
    {
        public string NewsId { get; set; }
        public string IpAddress { get; set; }
        public bool IsLiked { get; set; }
        public virtual News News { get; set; }
    }
}
