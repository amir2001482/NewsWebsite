using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Models
{
    public class PaginateModel
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
    }
}
