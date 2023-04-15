using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Home;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.Video;

namespace NewsWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uw;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public HomeController(IUnitOfWork uw, IHttpContextAccessor accessor, IMapper mapper, IConfiguration configuration)
        {
            _uw = uw;
            _accessor = accessor;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string TypeOfNews, string duration)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax && TypeOfNews == "MostViewedNews")
                return PartialView("_MostViewNews", await _uw.NewsRepository.MostViewedNewsAsync(0, 3, duration));

            else if (isAjax && TypeOfNews == "MostTalkNews")
                return PartialView("_MostTalkNews", await _uw.NewsRepository.MostTalkNewsAsync(0, 5, duration));
            else
            {
                var mostViewNews = await _uw.NewsRepository.MostViewedNewsAsync(0, 3, "day");
                var mostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 3, "day");
                //var mostPopularNews = await _uw.NewsRepository.MostPopularNewsAsync(0, 5);
                //var model = new PaginateModel
                //{
                //    offset = 0,
                //    limit = 10,
                //    orderBy = "PublishDateTime desc",
                //    searchText = "",
                //};
                var news = await _uw.NewsRepository.GetPaginateNewsAsync(new PaginateModel
                {
                    offset = 0,
                    limit = 10,
                    orderBy = "PublishDateTime desc",
                    searchText = "",
                }, true, null);
                var internalNews = await _uw.NewsRepository.GetPaginateNewsAsync(new PaginateModel
                {
                    offset = 0,
                    limit = 10,
                    orderBy = "PublishDateTime desc",
                    searchText = "",
                }, true, true);
                var forignNews = await _uw.NewsRepository.GetPaginateNewsAsync(new PaginateModel
                {
                    offset = 0,
                    limit = 10,
                    orderBy = "PublishDateTime desc",
                    searchText = "",
                }, true, false);
                var videosPaginateModel = new PaginateModel()
                {
                    offset = 0,
                    limit = 10,
                    orderBy = "PublishDateTime",
                    searchText = ""
                };
                var videos = await _uw.VideoRepository.GetPaginateVideosAsync(videosPaginateModel);
                var homePageViewModel = new HomePageViewModel(news, mostViewNews, mostTalkNews, internalNews, forignNews, videos,_uw.NewsRepository.GetPublishedNewsCount());
                return View(homePageViewModel);
            }
        }
        [Route("News/{newsId}/{url}")]
        public async Task<IActionResult> NewsDetails(string newsId, string url)
        {
            var currentuserId = User.Identity.GetUserId<int>();
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

            try
            {
                var news = await _uw.NewsRepository.GetNewsByIdAsync(newsId, currentuserId);
                var newsComments = await _uw.NewsRepository.GetNewsCommentsAsync(newsId);
                var nextAndPreviousNews = await _uw.NewsRepository.GetNextAndPreviousNews(news.PublishDateTime);
                var newsRelated = await _uw.NewsRepository.GetRelatedNewsAsync(2, news.TagIdsList, newsId);
                var newsDetailsViewModel = new NewsDetailsViewModel(news, newsComments, newsRelated, nextAndPreviousNews);
                return View(newsDetailsViewModel);
            }

            catch (Exception ex)
            {
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetNewsPaginate(int offset, int limit)
        {
            var model = new PaginateModel()
            {
                limit = limit,
                offset = offset,
                orderBy = "PublishDateTime Desc",
                searchText = ""
            };
            var news = await _uw.NewsRepository.GetPaginateNewsAsync(model, true, null);
            return PartialView("_NewsPaginate", new NewsPaginateViewModel(_uw.NewsRepository.GetPublishedNewsCount(), news));
        }

        [Route("Category/{categoryId}/{url}")]
        public async Task<IActionResult> NewsInCategory(string categoryId, string url)
        {
            if (!categoryId.HasValue())
                return NotFound();

            var category = await _uw.BaseRepository<Category>().FindByIdAsync(categoryId);
            if (category == null)
                return NotFound();
            return View("NewsInCategoryAndTag", new CategoryOrTagInfoViewModel { Id = category.CategoryId, Title = category.CategoryName, IsCategory = true, MostTalkNews = await _uw.NewsRepository.MostTalkNewsAsync(0, 5, "day") });
        }
        [Route("Tag/{tagId}")]
        public async Task<IActionResult> NewsInTag(string tagId)
        {
            if (!tagId.HasValue())
                return NotFound();
            var tag = await _uw.BaseRepository<Tag>().FindByIdAsync(tagId);
            if (tag == null)
                return NotFound();
            return View("NewsInCategoryAndTag", new CategoryOrTagInfoViewModel { Id = tag.TagId, Title = tag.TagName, IsCategory = false });
        }

        [HttpGet]
        public async Task<ActionResult> GetNewsInCategoryAndTag(int pageIndex, int pageSize, string categoryId, string tagId)
        {
            if (categoryId.HasValue())
                return Json(await _uw.NewsRepository.GetNewsInCategoryAsync(categoryId, pageIndex, pageSize));

            else
                return Json(await _uw.NewsRepository.GetNewsInTagAsync(tagId, pageIndex, pageSize));
        }

        [Route("Videos")]
        public async Task<IActionResult> Videos()
        {
            var videos = await _uw.BaseRepository<Video>().FindAllAsync();
            return View(videos);
        }
        [Route("Video/{videoId}")]
        public async Task<IActionResult> VideoDetails(string videoId)
        {
            if (!videoId.HasValue())
                return NotFound();
            else
            {
                var video = await _uw.BaseRepository<Video>().FindByIdAsync(videoId);
                if (video == null)
                    return NotFound();
                else
                    return View(video);
            }
        }

        [HttpGet]
        public async Task<JsonResult> LikeOrDisLike(string newsId, bool isLike)
        {
            string ipAddress = _accessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            Like likeOrDislike = _uw.BaseRepository<Like>().FindByConditionAsync(l => l.NewsId == newsId && l.IpAddress == ipAddress).Result.FirstOrDefault();
            if (likeOrDislike == null)
            {
                likeOrDislike = new Like { NewsId = newsId, IpAddress = ipAddress, IsLiked = isLike };
                await _uw.BaseRepository<Like>().CreateAsync(likeOrDislike);
            }
            else
                likeOrDislike.IsLiked = isLike;

            await _uw.Commit();
            var likeAndDislike = _uw.NewsRepository.NumberOfLikeAndDislike(newsId);
            return Json(new { like = likeAndDislike.NumberOfLike, dislike = likeAndDislike.NumberOfDisLike });
        }

        [HttpPost]
        public async Task<IActionResult> BookMark(string newsId)
        {
            var currentUserId = User.Identity.GetUserId<int>();
            if (!User.Identity.IsAuthenticated)
                return PartialView("_SignIn");
            return Json(await _uw.NewsRepository.BookMarkAsync(newsId, currentUserId));
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            var news = await _uw.NewsRepository.Search(searchText , 0 , 5);
            var res = new HomePageViewModel(news, null, null, null, null, null, news.Count());
            return View(res);
        }

        [HttpGet]
        public async Task<IActionResult> SearchPaginate( string searchText, int offset, int limit)
        {
            var news = await _uw.NewsRepository.Search(searchText, offset, limit);
            return PartialView("_NewsPaginate", new NewsPaginateViewModel(news.Count(), news));
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Error404()
        {
            return View();
        }

    }
}