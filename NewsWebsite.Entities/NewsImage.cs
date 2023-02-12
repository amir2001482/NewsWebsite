using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsWebsite.Entities
{
    public class NewsImage
    {
        [Key]
        public string NewsImageId { get; set; }
        public string NewsId { get; set; }
        public string ImageName { get; set; }
        public virtual News News { get; set; }
       

    }
}
