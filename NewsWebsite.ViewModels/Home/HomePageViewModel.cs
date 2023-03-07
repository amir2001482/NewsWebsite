using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Home
{
    public class HomePageViewModel
    {
        public HomePageViewModel(List<NewsViewModel> news)
        {
            _news = news;
        }
        public List<NewsViewModel> _news { get; set; }
    }
}
