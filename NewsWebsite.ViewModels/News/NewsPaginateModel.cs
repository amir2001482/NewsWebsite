using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.News
{
    public class NewsPaginateModel
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public Func<NewsViewModel, object> orderByAsc { get; set; }
        public Func<NewsViewModel, object> orderByDes { get; set; }
        public string searchText { get; set; }
        public bool? isPublish { get; set; }
        public bool? isInternal { get; set; }

    }
}
