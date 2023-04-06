using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Home
{
    public class CategoryOrTagInfoViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsCategory { get; set; }
    }
}
