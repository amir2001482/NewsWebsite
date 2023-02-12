using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Entities
{
    public class NewsCategory
    {
        public string NewsId { get; set; }
        public string CategoryId { get; set; }

        public virtual News News { get; set; }
        public virtual Category Category { get; set; }
    }
}
