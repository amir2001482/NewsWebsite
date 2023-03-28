using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewComponents
{
    public class RandomNewsList : ViewComponent
    {
        private readonly IUnitOfWork _uw;
        public RandomNewsList(IUnitOfWork uw)
        {
            _uw = uw;
        }

        public async Task<IViewComponentResult> InvokeAsync(int number)
        {
            var newsList = new List<NewsViewModel>();
            int randomRow;
            for (int i=0;i<number;i++)
            {
                randomRow = CustomMethods.RandomNumber(1, _uw.NewsRepository.GetPublishedNewsCount()+1);
                var news = await _uw._Context.News.Where(n => n.IsPublish == true && n.PublishDateTime <= DateTime.Now).Select(n => new NewsViewModel { Title = n.Title, Url = n.Url, NewsId = n.NewsId, ImageName = n.ImageName }).Skip(randomRow-1).Take(1).FirstOrDefaultAsync();
                newsList.Add(news);
            }
           
            return View(newsList);
        }
    }
}
