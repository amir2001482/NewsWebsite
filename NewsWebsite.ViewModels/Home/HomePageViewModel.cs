using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.Video;
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
            List<NewsViewModel> forignNews,
            List<VideoViewModel> videos ,
            int countNewsPublished)
        {
            News = news;
            MostViewNews = mostViewNews;
            MostTalkNews = mostTalkNews;
            MostPopularNews = mostPopularNews;
            InternalNews = internalNews;
            ForignNews = forignNews;
            Videos = videos;
            CountNewsPublished = countNewsPublished;
        }
        public List<NewsViewModel> News { get; set; }
        public List<NewsViewModel> MostViewNews { get; set; }
        public List<NewsViewModel> MostTalkNews { get; set; }
        public List<NewsViewModel> MostPopularNews { get; set; }
        public List<NewsViewModel> InternalNews { get; set; }
        public List<NewsViewModel> ForignNews { get; set; }
        public List<VideoViewModel> Videos { get; set; }
        public int CountNewsPublished { get; set; }
    }
}
