using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewComponents
{
    public class MostTalkNews :  ViewComponent
    {
        private readonly IUnitOfWork _uw;
        public MostTalkNews(IUnitOfWork uw)
        {
            _uw = uw;
        }
        public async Task<IViewComponentResult> InvokeAsync(string duration)
        {
            var mostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 3, duration);
            return View(mostTalkNews);
        }

        //public async Task<IViewComponentResult> MostTalkNewsDuration(string duration)
        //{
        //    var mostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 3, duration);
        //    return View(mostTalkNews);
        //}
    }
}
