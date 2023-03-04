using NewsWebsite.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.News
{
    public class NewsCategoriesViewModel
    {
        public NewsCategoriesViewModel(List<TreeViewCategory> categories, string[] categoryId)
        {
            Categories = categories;
            CategoryId = categoryId;
        }

        public List<TreeViewCategory> Categories { get; set; }
        public string[] CategoryId { get; set; }
    }
}
