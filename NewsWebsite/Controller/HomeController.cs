using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Home;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.Video;

namespace NewsWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _accessor;

        public HomeController(IUnitOfWork uw, IHttpContextAccessor accessor)
        {
            _uw = uw;
            _accessor = accessor;
        }

        public async Task<IActionResult> Index(string TypeOfNews, string duration)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax && TypeOfNews == "MostViewedNews")
                return PartialView("_MostViewNews", await _uw.NewsRepository.MostViewedNewsAsync(0, 3, duration));

            else if (isAjax && TypeOfNews == "MostTalkNews")
                return PartialView("_MostTalkNews", await _uw.NewsRepository.MostTalkNewsAsync(0, 3, duration));
            else
            {
                var newsPaginateModel = new NewsPaginateModel()
                {
                    offset = 0,
                    limit = 10,
                    orderByAsc = item => "",
                    orderByDes = item => item.PersianPublishDate,
                    searchText = "",
                    isPublish = true
                };
                var news = await _uw.NewsRepository.GetPaginateNewsAsync(newsPaginateModel);
                var mostViewNews = await _uw.NewsRepository.MostViewedNewsAsync(0, 3, "day");
                var mostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 3, "day");
                var mostPopularNews = await _uw.NewsRepository.MostPopularNewsAsync(0, 5);
                newsPaginateModel.isInternal = true;
                var internalNews = await _uw.NewsRepository.GetPaginateNewsAsync(newsPaginateModel);
                newsPaginateModel.isInternal = false;
                var forignNews = await _uw.NewsRepository.GetPaginateNewsAsync(newsPaginateModel);
                var videos = await _uw.VideoRepository.GetPaginateVideosAsync(new VideoPaginateModel
                {
                    offset = 0,
                    limit = 10,
                    titleSortAsc = null,
                    publishDateTimeSortAsc = false,
                    searchText = ""
                });
                var homePageViewModel = new HomePageViewModel(news, mostViewNews, mostTalkNews, mostPopularNews, internalNews, forignNews, videos , _uw.NewsRepository.GetPublishedNewsCount());
                return View(homePageViewModel);
            }
        }
        [Route("News/{newsId}/{url}")]
        public async Task<IActionResult> NewsDetails(string newsId, string url)
        {
            string ipAddress = _accessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            Visit visit = _uw.BaseRepository<Visit>().FindByConditionAsync(n => n.NewsId == newsId && n.IpAddress == ipAddress).Result.FirstOrDefault();
            if (visit != null && visit.LastVisitDateTime.Date != DateTime.Now.Date)
            {
                visit.NumberOfVisit = visit.NumberOfVisit + 1;
                visit.LastVisitDateTime = DateTime.Now;
                await _uw.Commit();
            }
            else if (visit == null)
            {
                visit = new Visit { IpAddress = ipAddress, LastVisitDateTime = DateTime.Now, NewsId = newsId, NumberOfVisit = 1 };
                await _uw.BaseRepository<Visit>().CreateAsync(visit);
                await _uw.Commit();
            }

            var news = await _uw.NewsRepository.GetNewsById(newsId);
            var newsComments = await _uw.NewsRepository.GetNewsCommentsAsync(newsId);
            var nextAndPreviousNews = await _uw.NewsRepository.GetNextAndPreviousNews(news.PublishDateTime);
            var newsRelated = await _uw.NewsRepository.GetRelatedNews(2, news.TagIdsList, newsId);
            var newsDetailsViewModel = new NewsDetailsViewModel(news, newsComments, newsRelated, nextAndPreviousNews);
            return View(newsDetailsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsPaginate(int offset , int limit)
        {
            try
            {
                var news = await _uw.NewsRepository.GetPaginateNewsAsync(new NewsPaginateModel { limit = limit, offset = offset ,  orderByAsc = item => "" , orderByDes = item => item.PersianPublishDate , isPublish = true , searchText = "" });
                return PartialView("_NewsPaginate", new NewsPaginateViewModel(_uw.NewsRepository.GetPublishedNewsCount() , news));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
    }
}