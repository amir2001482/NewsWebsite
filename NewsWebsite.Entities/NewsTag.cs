using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Entities
{
    public class NewsTag
    {
        public string NewsId { get; set; }
        public string TagId { get; set; }
        public virtual News News { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
