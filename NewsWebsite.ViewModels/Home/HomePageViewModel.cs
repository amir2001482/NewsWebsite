using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Home
{
    public class HomePageViewModel
    {
        public HomePageViewModel(List<NewsViewModel> news , List<NewsViewModel> mostViewNews)
        {
            News = news;
            MostViewNews = mostViewNews;
        }
        public List<NewsViewModel> News { get; set; }
        public List<NewsViewModel> MostViewNews { get; set; }
    }
}
