using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewsWebsite.Entities
{
    public class Category
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        [ForeignKey("Parent")]
        public string ParentCategoryId { get; set; }
        public string Url { get; set; }
       
        public Category Parent { get; set; }
        public List<Category> categories { get; set; }
        public virtual ICollection<NewsCategory> NewsCategories { get; set; }

    }
}
