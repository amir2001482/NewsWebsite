using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewComponents
{
    public class MostPopularNews : ViewComponent
    {
        private readonly IUnitOfWork _uw;
        public MostPopularNews(IUnitOfWork uw)
        {
            _uw = uw;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mostPopularNews = await _uw.NewsRepository.MostPopularNewsAsync(0, 5);
            return View(mostPopularNews);
        }
    }
}
