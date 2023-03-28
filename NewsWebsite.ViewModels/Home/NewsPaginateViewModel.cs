using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Home
{
    public class NewsPaginateViewModel
    {
        public NewsPaginateViewModel(int newscount , List<NewsViewModel> news)
        {
            NewsCount = newscount;
            News = news;
        }
        public int NewsCount { get; set; }
        public List<NewsViewModel> News { get; set; }
    }
}
