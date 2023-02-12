using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Entities
{
    public class Visit
    {
        public string IpAddress { get; set; }
        public string NewsId { get; set; }
        public DateTime LastVisitDateTime { get; set; }
        public int NumberOfVisit { get; set; }
        public virtual News News { get; set; }

    }
}
