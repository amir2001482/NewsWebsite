using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.Home;
using NewsWebsite.ViewModels.News;

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
                var model = new NewsPaginateModel()
                {
                     offset = 0,
                     limit = 10,
                     orderByAsc = item=> "",
                     orderByDes = item=> item.PersianPublishDate,
                     searchText = "",
                     isPublish = true
                };
                var news = await _uw.NewsRepository.GetPaginateNewsAsync(model);
                var mostViewNews = await _uw.NewsRepository.MostViewedNewsAsync(0, 3, "day");
                var mostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 3, "day");
                var mostPopularNews = await _uw.NewsRepository.MostPopularNewsAsync(0 , 5);
                model.isInternal = true;
                var internalNews = await _uw.NewsRepository.GetPaginateNewsAsync(model);
                model.isInternal = false;
                var forignNews = await _uw.NewsRepository.GetPaginateNewsAsync(model);
                var homePageViewModel = new HomePageViewModel(news, mostViewNews , mostTalkNews , mostPopularNews , internalNews , forignNews);
                return View(homePageViewModel);
            }
        }
    }
}