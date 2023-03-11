using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.Home;

namespace NewsWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uw;

        public HomeController(IUnitOfWork uw)
        {
            _uw = uw;
        }
       
        public async Task<IActionResult> Index(string TypeOfNews , string duration)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax && TypeOfNews == "MostViewedNews")
                return PartialView("_MostViewNews", await _uw.NewsRepository.MostViewedNewsAsync(0, 3, duration));

            else if(isAjax && TypeOfNews == "MostTalkNews")
                return PartialView("_MostTalkNews", await _uw.NewsRepository.MostTalkNewsAsync(0, 3, duration));
            else
            {
                var news = await _uw.NewsRepository.GetPaginateNewsAsync(0, 10, item => "", item => item.PersianPublishDate, "", true);
                var mostViewNews = await _uw.NewsRepository.MostViewedNewsAsync(0, 3, "day");
                var mostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 3, "day");
                var mostPopularNews = await _uw.NewsRepository.MostPopularNewsAsync(0 , 5);
                var homePageViewModel = new HomePageViewModel(news, mostViewNews , mostTalkNews , mostPopularNews);
                return View(homePageViewModel);
            }
        }
    }
}