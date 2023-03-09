using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewComponents
{
    public class LatestNewsTitleList : ViewComponent
    {
        private readonly IUnitOfWork _uw;
        public LatestNewsTitleList(IUnitOfWork uw)
        {
            _uw = uw;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestNews = await _uw._Context.News
                .Where(c => c.IsPublish == true && c.PublishDateTime <= DateTime.Now)
                .Select(c => new NewsViewModel { Title = c.Title, Url = c.Url, NewsId = c.NewsId }).ToListAsync();

            return View(latestNews);
        }
    }
}
