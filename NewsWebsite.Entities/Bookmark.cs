using NewsWebsite.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Entities
{
    public class Bookmark
    {
        public string NewsId { get; set; }
        public string UserId { get; set; }
        public virtual News News { get; set; }
        public virtual User User { get; set; }

    }
}
