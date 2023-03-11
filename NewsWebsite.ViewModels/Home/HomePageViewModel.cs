using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Home
{
    public class HomePageViewModel
    {
        public HomePageViewModel(List<NewsViewModel> news ,
            List<NewsViewModel> mostViewNews ,
            List<NewsViewModel> mostTalkNews ,
            List<NewsViewModel> mostPopularNews,
            List<NewsViewModel> internalNews,
            List<NewsViewModel> forignNews)
        {
            News = news;
            MostViewNews = mostViewNews;
            MostTalkNews = mostTalkNews;
            MostPopularNews = mostPopularNews;
            InternalNews = internalNews;
            ForignNews = forignNews;
        }
        public List<NewsViewModel> News { get; set; }
        public List<NewsViewModel> MostViewNews { get; set; }
        public List<NewsViewModel> MostTalkNews { get; set; }
        public List<NewsViewModel> MostPopularNews { get; set; }
        public List<NewsViewModel> InternalNews { get; set; }
        public List<NewsViewModel> ForignNews { get; set; }
    }
}
