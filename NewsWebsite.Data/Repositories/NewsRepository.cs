﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Home;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using NewsWebsite.Entities.identity;

namespace NewsWebsite.Data.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _uw;
        public NewsRepository(NewsDBContext context, IMapper mapper, IConfiguration configuration , IUnitOfWork uw)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));

            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(_configuration));
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
        }


        //public async Task<List<NewsViewModel>> GetPaginateNewsAsync(int offset, int limit, bool? titleSortAsc, bool? visitSortAsc, bool? likeSortAsc, bool? dislikeSortAsc, bool? publishDateTimeSortAsc, string searchText)
        //{
        //    string NameOfCategories = "";
        //    string NameOfTags = "";
        //    List<NewsViewModel> newsViewModel = new List<NewsViewModel>();
        //    // chon az join estefade mikonim date tekrari khahim dasht banabarin dar akhar az group by estefade karde ta data hay tekrari ra hazf konim
        //    var newsGroup = await (from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(u=>u.User)
        //                           join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
        //                           from bct in bc.DefaultIfEmpty()
        //                           join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
        //                           from cog in cg.DefaultIfEmpty()
        //                           join a in _context.NewsTags on n.NewsId equals a.NewsId into ac
        //                           from act in ac.DefaultIfEmpty()
        //                           join t in _context.Tags on act.TagId equals t.TagId into tg
        //                           from tog in tg.DefaultIfEmpty()
        //                           where (n.Title.Contains(searchText))
        //                           select (new
        //                           {
        //                               n.NewsId,
        //                               n.Title,
        //                               ShortTitle = n.Title.Length > 60 ? n.Title.Substring(0, 60) + "..." : n.Title,
        //                               n.Url,
        //                               n.Description,
        //                               NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
        //                               NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
        //                               NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
        //                               CategoryName = cog != null ? cog.CategoryName : "",
        //                               TagName= tog!=null ? tog.TagName :"",
        //                               AuthorName=n.User.FirstName+" "+ n.User.LastName,
        //                               n.IsPublish,
        //                               NewsType = n.IsInternal == true ? "داخلی" : "خارجی",
        //                               PublishDateTime=n.PublishDateTime==null? new DateTime(01,01,01):n.PublishDateTime,
        //                               PersianPublishDateTime = n.PublishDateTime==null?"-": n.PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss"),
        //                           })).GroupBy(b => b.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g }).Skip(offset).Take(limit).AsNoTracking().ToListAsync();

        //    foreach (var item in newsGroup)
        //    {
        //        NameOfCategories = "";
        //        NameOfTags = "";
        //        // ba estefade az halghe ha  dar group ha pimaiesh karde va data hay tekrari ra hazf 
        //        foreach (var a in item.NewsGroup.Select(a => a.CategoryName).Distinct())
        //        {
        //            if (NameOfCategories == "")
        //                NameOfCategories = a;
        //            else
        //                NameOfCategories = NameOfCategories + " - " + a;
        //        }

        //        foreach (var a in item.NewsGroup.Select(a => a.TagName).Distinct())
        //        {
        //            if (NameOfTags == "")
        //                NameOfTags = a;
        //            else
        //                NameOfTags = NameOfTags + " - " + a;
        //        }

        //        NewsViewModel news = new NewsViewModel()
        //        {
        //            NewsId = item.NewsId,
        //            Title = item.NewsGroup.First().Title,
        //            ShortTitle = item.NewsGroup.First().ShortTitle,
        //            Url = item.NewsGroup.First().Url,
        //            Description = item.NewsGroup.First().Description,
        //            NumberOfVisit = item.NewsGroup.First().NumberOfVisit,
        //            NumberOfDisLike = item.NewsGroup.First().NumberOfDisLike,
        //            NumberOfLike = item.NewsGroup.First().NumberOfLike,
        //            PersianPublishDate = item.NewsGroup.First().PersianPublishDateTime,
        //            NewsType = item.NewsGroup.First().NewsType,
        //            Status = item.NewsGroup.First().IsPublish==false?"پیش نویس": (item.NewsGroup.First().PublishDateTime > DateTime.Now ? "انتشار در آینده" : "منتشر شده"),
        //            NameOfCategories = NameOfCategories,
        //            NameOfTags = NameOfTags,
        //            AuthorName = item.NewsGroup.First().AuthorName,
        //        };
        //        newsViewModel.Add(news);
        //    }

        //    if (titleSortAsc != null)
        //        newsViewModel = newsViewModel.OrderBy(c => (titleSortAsc == true && titleSortAsc != null) ? c.Title : "")
        //                             .OrderByDescending(c => (titleSortAsc == false && titleSortAsc != null) ? c.Title : "").ToList();

        //    else if (visitSortAsc != null)
        //        newsViewModel = newsViewModel.OrderBy(c => (visitSortAsc == true && visitSortAsc != null) ? c.NumberOfVisit : 0)
        //                           .OrderByDescending(c => (visitSortAsc == false && visitSortAsc != null) ? c.NumberOfVisit : 0).ToList();

        //    else if (likeSortAsc != null)
        //        newsViewModel = newsViewModel.OrderBy(c => (likeSortAsc == true && likeSortAsc != null) ? c.NumberOfLike : 0)
        //                           .OrderByDescending(c => (likeSortAsc == false && likeSortAsc != null) ? c.NumberOfLike : 0).ToList();

        //    else if (dislikeSortAsc != null)
        //        newsViewModel = newsViewModel.OrderBy(c => (dislikeSortAsc == true && dislikeSortAsc != null) ? c.NumberOfDisLike : 0)
        //                           .OrderByDescending(c => (dislikeSortAsc == false && dislikeSortAsc != null) ? c.NumberOfDisLike : 0).ToList();

        //    else if (publishDateTimeSortAsc != null)
        //        newsViewModel = newsViewModel.OrderBy(c => (publishDateTimeSortAsc == true && publishDateTimeSortAsc != null) ? c.PersianPublishDate : "")
        //                           .OrderByDescending(c => (publishDateTimeSortAsc == false && publishDateTimeSortAsc != null) ? c.PersianPublishDate : "").ToList();

        //    foreach (var item in newsViewModel)
        //        item.Row = ++offset;

        //    return newsViewModel;

        //}

        public int CountNews() => _context.News.Count();
        public int CountFuturePublishedNews() => _context.News.Where(n => n.PublishDateTime > DateTime.Now).Count();
        public int CountNewsPublishedOrDraft(bool isPublish) => _context.News.Where(n => isPublish ? n.IsPublish && n.PublishDateTime <= DateTime.Now : !n.IsPublish).Count();
        public int CountNewsPublished() => _context.News.Where(n => n.IsPublish == true && n.PublishDateTime <= DateTime.Now).Count();

        public string CheckNewsFileName(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            int fileNameCount = _context.News.Where(f => f.ImageName == fileName).Count();
            int j = 1;
            while (fileNameCount != 0)
            {
                fileName = fileName.Replace(fileExtension, "") + j + fileExtension;
                fileNameCount = _context.Videos.Where(f => f.Poster == fileName).Count();
                j++;
            }

            return fileName;
        }

        public async Task<List<NewsViewModel>> GetPaginateNewsAsync(PaginateModel model, bool? isPublish, bool? isInternal)
        {
            string NameOfCategories = "";
            string NameOfTags = "";
            List<NewsViewModel> newsViewModel = new List<NewsViewModel>();
            var getDateTimesForSearch = model.searchText.GetStartAndEndDateForSearch();

            var convertPublish = Convert.ToBoolean(isPublish);
            var convertInternal = Convert.ToBoolean(isInternal);
            var allNews = await (from n in ((from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(u => u.User).Include(c => c.Comments)
                                             where ((n.Title.Contains(model.searchText) || (n.PublishDateTime >= getDateTimesForSearch.First() && n.PublishDateTime <= getDateTimesForSearch.Last())) && (isInternal == null || (convertInternal ? n.IsInternal : !n.IsInternal)) && (isPublish == null || (convertPublish ? n.IsPublish && n.PublishDateTime <= DateTime.Now : !n.IsPublish)))
                                             select (new
                                             {
                                                 n.NewsId,
                                                 n.Title,
                                                 n.Abstract,
                                                 ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                                 n.Url,
                                                 n.ImageName,
                                                 n.Description,
                                                 NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                 NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                                 NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                                 NumberOfComments = n.Comments.Count(),
                                                 AuthorName = n.User.FirstName + " " + n.User.LastName,
                                                 n.IsPublish,
                                                 NewsType = n.IsInternal == true ? "داخلی" : "خارجی",
                                                 n.PublishDateTime,
                                             })).OrderBy(model.orderBy).Skip(model.offset).Take(model.limit))
                                 join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
                                 from bct in bc.DefaultIfEmpty()
                                 join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                                 from cog in cg.DefaultIfEmpty()
                                 join a in _context.NewsTags on n.NewsId equals a.NewsId into ac
                                 from act in ac.DefaultIfEmpty()
                                 join t in _context.Tags on act.TagId equals t.TagId into tg
                                 from tog in tg.DefaultIfEmpty()
                                 select (new NewsViewModel
                                 {
                                     NewsId = n.NewsId,
                                     Title = n.Title,
                                     Abstract = n.Abstract,
                                     ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                     Url = n.Url,
                                     ImageName = n.ImageName,
                                     Description = n.Description,
                                     NumberOfVisit = n.NumberOfVisit,
                                     NumberOfLike = n.NumberOfLike,
                                     NumberOfDisLike = n.NumberOfDisLike,
                                     NumberOfComment = n.NumberOfComments,
                                     AuthorName = n.AuthorName,
                                     IsPublish = n.IsPublish,
                                     NewsType = n.NewsType,
                                     PublishDateTime = n.PublishDateTime,
                                     NameOfCategories = cog != null ? cog.CategoryName : "",
                                     NameOfTags = tog != null ? tog.TagName : "",
                                 })).AsNoTracking().ToListAsync();


            var newsGroup = allNews.GroupBy(g => g.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g });
            foreach (var item in newsGroup)
            {
                NameOfCategories = "";
                NameOfTags = "";
                foreach (var a in item.NewsGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                foreach (var a in item.NewsGroup.Select(a => a.NameOfTags).Distinct())
                {
                    if (NameOfTags == "")
                        NameOfTags = a;
                    else
                        NameOfTags = NameOfTags + " - " + a;
                }

                NewsViewModel news = new NewsViewModel()
                {
                    NewsId = item.NewsId,
                    Title = item.NewsGroup.First().Title,
                    ShortTitle = item.NewsGroup.First().ShortTitle,
                    Abstract = item.NewsGroup.First().Abstract,
                    Url = item.NewsGroup.First().Url,
                    Description = item.NewsGroup.First().Description,
                    NumberOfVisit = item.NewsGroup.First().NumberOfVisit,
                    NumberOfDisLike = item.NewsGroup.First().NumberOfDisLike,
                    NumberOfLike = item.NewsGroup.First().NumberOfLike,
                    NewsType = item.NewsGroup.First().NewsType,
                    Status = item.NewsGroup.First().IsPublish == false ? "پیش نویس" : (item.NewsGroup.First().PublishDateTime > DateTime.Now ? "انتشار در آینده" : "منتشر شده"),
                    NameOfCategories = NameOfCategories,
                    NameOfTags = NameOfTags,
                    ImageName = item.NewsGroup.First().ImageName,
                    AuthorName = item.NewsGroup.First().AuthorName,
                    NumberOfComment = item.NewsGroup.First().NumberOfComment,
                    PublishDateTime = item.NewsGroup.First().PublishDateTime,
                    PersianPublishDate = item.NewsGroup.First().PublishDateTime == null ? "-" : DateTimeExtensions.ConvertMiladiToShamsi(item.NewsGroup.First().PublishDateTime, "yyyy/MM/dd ساعت HH:mm"),
                };
                newsViewModel.Add(news);
            }

            foreach (var item in newsViewModel)
                item.Row = ++model.offset;

            return newsViewModel;

        }

        public async Task<List<NewsViewModel>> MostViewedNewsAsync(int offset, int limit, string duration)
        {
            string NameOfCategories = "";
            List<NewsViewModel> newsViewModel = new List<NewsViewModel>();
            DateTime StartMiladiDate;
            DateTime EndMiladiDate = DateTime.Now;

            if (duration == "week")
            {
                int NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
                StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            }

            else if (duration == "day")
                StartMiladiDate = DateTime.Now.Date + new TimeSpan(0, 0, 0);

            else
            {
                string DayOfMonth = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dd").Fa2En();
                StartMiladiDate = DateTime.Now.AddDays((-1) * (int.Parse(DayOfMonth) - 1)).Date + new TimeSpan(0, 0, 0);
            }

            var allNews = await (from n in ((from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(c => c.Comments)
                                             where (n.PublishDateTime <= EndMiladiDate && StartMiladiDate <= n.PublishDateTime)
                                             select (new
                                             {
                                                 n.NewsId,
                                                 ShortTitle = n.Title.Length > 60 ? n.Title.Substring(0, 60) + "..." : n.Title,
                                                 n.Url,
                                                 NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                 NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                                 NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                                 NumberOfComments = n.Comments.Count(),
                                                 n.ImageName,
                                                 PublishDateTime = n.PublishDateTime == null ? new DateTime(01, 01, 01) : n.PublishDateTime,
                                             })).OrderBy("NumberOfVisit desc").Skip(offset).Take(limit))
                                 join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
                                 from bct in bc.DefaultIfEmpty()
                                 join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                                 from cog in cg.DefaultIfEmpty()
                                 select (new
                                 {
                                     n.NewsId,
                                     n.ShortTitle,
                                     n.Url,
                                     n.NumberOfVisit,
                                     n.NumberOfLike,
                                     n.NumberOfDisLike,
                                     n.NumberOfComments,
                                     n.ImageName,
                                     CategoryName = cog != null ? cog.CategoryName : "",
                                     n.PublishDateTime,
                                 })).AsNoTracking().ToListAsync();

            var newsGroup = allNews.GroupBy(g => g.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g });
            foreach (var item in newsGroup)
            {
                NameOfCategories = "";
                foreach (var a in item.NewsGroup.Select(a => a.CategoryName).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                NewsViewModel news = new NewsViewModel()
                {
                    NewsId = item.NewsId,
                    ShortTitle = item.NewsGroup.First().ShortTitle,
                    Url = item.NewsGroup.First().Url,
                    NumberOfVisit = item.NewsGroup.First().NumberOfVisit,
                    NumberOfDisLike = item.NewsGroup.First().NumberOfDisLike,
                    NumberOfLike = item.NewsGroup.First().NumberOfLike,
                    NameOfCategories = NameOfCategories,
                    PublishDateTime = item.NewsGroup.First().PublishDateTime,
                    ImageName = item.NewsGroup.First().ImageName,
                };
                newsViewModel.Add(news);
            }

            return newsViewModel;
        }

        public async Task<List<NewsViewModel>> MostTalkNewsAsync(int offset, int limit, string duration)
        {
            DateTime StartMiladiDate;
            DateTime EndMiladiDate = DateTime.Now;

            if (duration == "week")
            {
                int NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
                StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            }

            else if (duration == "day")
                StartMiladiDate = DateTime.Now.Date + new TimeSpan(0, 0, 0);

            else
            {
                string DayOfMonth = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dd").Fa2En();
                StartMiladiDate = DateTime.Now.AddDays((-1) * (int.Parse(DayOfMonth) - 1)).Date + new TimeSpan(0, 0, 0);
            }

            return await (from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(c => c.Comments)
                          where (n.PublishDateTime <= EndMiladiDate && StartMiladiDate <= n.PublishDateTime)
                          select (new NewsViewModel
                          {
                              NewsId = n.NewsId,
                              ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                              Url = n.Url,
                              NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                              NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                              NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                              NumberOfComment = n.Comments.Count(),
                              ImageName = n.ImageName,
                              PublishDateTime = n.PublishDateTime == null ? new DateTime(01, 01, 01) : n.PublishDateTime,
                          })).OrderByDescending(o => o.NumberOfComment).Skip(offset).Take(limit).AsNoTracking().ToListAsync();
        }

        public async Task<List<NewsViewModel>> MostPopularNewsAsync(int offset, int limit)
        {
            var newsList = await _context.News
               .Include(e => e.Comments)
               .Include(e => e.Likes)
               .Include(e => e.User)
               .Include(e => e.Visits)
               .Include(e => e.NewsTags).ThenInclude(d => d.Tag)
               .Include(e => e.NewsCategories).ThenInclude(d => d.Category)
               .AsNoTracking().ToListAsync();
            var res = SetCategoryAndTagNames(_mapper.Map<List<NewsViewModel>>(newsList), offset);
            return res.OrderByDescending(d => d.NumberOfLike).Skip(offset).Take(limit).ToList();

        }
        public async Task<NewsViewModel> GetNewsByIdAsync(string newsId, int userId)
        {
            string NameOfCategories = "";
            var news = new NewsViewModel();
            var newsInfo = await (from n in _context.News.Where(n => n.NewsId == newsId).Include(v => v.Visits).Include(l => l.Likes).Include(u => u.User).Include(c => c.Comments)
                                  join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
                                  from bct in bc.DefaultIfEmpty()
                                  join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                                  from cog in cg.DefaultIfEmpty()
                                  join a in _context.NewsTags on n.NewsId equals a.NewsId into ac
                                  from act in ac.DefaultIfEmpty()
                                  join t in _context.Tags on act.TagId equals t.TagId into tg
                                  from tog in tg.DefaultIfEmpty()
                                  select (new NewsViewModel
                                  {
                                      NewsId = n.NewsId,
                                      Title = n.Title,
                                      Abstract = n.Abstract,
                                      ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                      Url = n.Url,
                                      ImageName = n.ImageName,
                                      Description = n.Description,
                                      NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                      NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                      NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                      NumberOfComment = n.Comments.Where(c => c.IsConfirm == true).Count(),
                                      NameOfCategories = cog != null ? cog.CategoryName : "",
                                      NameOfTags = tog != null ? tog.TagName : "",
                                      IdOfTags = tog != null ? tog.TagId : "",
                                      AuthorInfo = n.User,
                                      IsPublish = n.IsPublish,
                                      NewsType = n.IsInternal == true ? "داخلی" : "خارجی",
                                      PublishDateTime = n.PublishDateTime == null ? new DateTime(01, 01, 01) : n.PublishDateTime,
                                      PersianPublishDate = n.PublishDateTime == null ? "-" : n.PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss"),
                                      Bookmarked = n.Bookmarks.Any(b => b.UserId == userId && b.NewsId == newsId),
                                  })).AsNoTracking().ToListAsync();
            if(newsInfo.Count() > 0)
            {
                var newsGroup = newsInfo.GroupBy(g => g.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g });
                foreach (var a in newsGroup.First().NewsGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                news = new NewsViewModel()
                {
                    NewsId = newsGroup.First().NewsGroup.First().NewsId,
                    Title = newsGroup.First().NewsGroup.First().Title,
                    ShortTitle = newsGroup.First().NewsGroup.First().ShortTitle,
                    Abstract = newsGroup.First().NewsGroup.First().Abstract,
                    Url = newsGroup.First().NewsGroup.First().Url,
                    Description = newsGroup.First().NewsGroup.First().Description,
                    NumberOfVisit = newsGroup.First().NewsGroup.First().NumberOfVisit,
                    NumberOfDisLike = newsGroup.First().NewsGroup.First().NumberOfDisLike,
                    NumberOfLike = newsGroup.First().NewsGroup.First().NumberOfLike,
                    PersianPublishDate = newsGroup.First().NewsGroup.First().PersianPublishDate,
                    NewsType = newsGroup.First().NewsGroup.First().NewsType,
                    Status = newsGroup.First().NewsGroup.First().IsPublish == false ? "پیش نویس" : (newsGroup.First().NewsGroup.First().PublishDateTime > DateTime.Now ? "انتشار در آینده" : "منتشر شده"),
                    NameOfCategories = NameOfCategories,
                    TagNamesList = newsGroup.First().NewsGroup.Select(a => a.NameOfTags).Distinct().ToList(),
                    TagIdsList = newsGroup.First().NewsGroup.Select(a => a.IdOfTags).Distinct().ToList(),
                    ImageName = newsGroup.First().NewsGroup.First().ImageName,
                    AuthorInfo = newsGroup.First().NewsGroup.First().AuthorInfo,
                    NumberOfComment = newsGroup.First().NewsGroup.First().NumberOfComment,
                    PublishDateTime = newsGroup.First().NewsGroup.First().PublishDateTime,
                    Bookmarked = newsGroup.First().NewsGroup.First().Bookmarked,
                };
            }
            return news;
        }
        public async Task<List<NewsViewModel>> GetNextAndPreviousNews(DateTime? PublishDateTime)
        {
            var newsList = new List<NewsViewModel>();
            newsList.Add(await (from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(c => c.Comments)
                                where (n.IsPublish == true && n.PublishDateTime <= DateTime.Now && n.PublishDateTime < PublishDateTime)
                                select (new NewsViewModel
                                {
                                    NewsId = n.NewsId,
                                    ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                    Url = n.Url,
                                    Title = n.Title,
                                    NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                    NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                    NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                    NumberOfComment = n.Comments.Count(),
                                    ImageName = n.ImageName,
                                    PublishDateTime = n.PublishDateTime == null ? new DateTime(01, 01, 01) : n.PublishDateTime,
                                })).OrderByDescending(o => o.PublishDateTime).AsNoTracking().FirstOrDefaultAsync());

            newsList.Add(await (from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(c => c.Comments)
                                where (n.IsPublish == true && n.PublishDateTime <= DateTime.Now && n.PublishDateTime > PublishDateTime)
                                select (new NewsViewModel
                                {
                                    NewsId = n.NewsId,
                                    ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                    Url = n.Url,
                                    Title = n.Title,
                                    NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                    NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                    NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                    NumberOfComment = n.Comments.Count(),
                                    ImageName = n.ImageName,
                                    PublishDateTime = n.PublishDateTime == null ? new DateTime(01, 01, 01) : n.PublishDateTime,
                                })).OrderBy(o => o.PublishDateTime).AsNoTracking().FirstOrDefaultAsync());

            return newsList;
        }

        public async Task<List<Comment>> GetNewsCommentsAsync(string newsId)
        {
            var comments = await (from c in _context.Comments
                                  where (c.ParentCommentId == null && c.NewsId == newsId && c.IsConfirm == true)
                                  select new Comment { CommentId = c.CommentId, Desription = c.Desription, Email = c.Email, PostageDateTime = c.PostageDateTime, Name = c.Name, IsConfirm = c.IsConfirm }).ToListAsync();
            foreach (var item in comments)
                await BindSubComments(item);

            return comments;
        }

        public async Task BindSubComments(Comment comment)
        {
            var subComments = await (from c in _context.Comments
                                     where (c.ParentCommentId == comment.CommentId && c.IsConfirm == true)
                                     select new Comment { CommentId = c.CommentId, Desription = c.Desription, Email = c.Email, PostageDateTime = c.PostageDateTime, Name = c.Name, IsConfirm = c.IsConfirm }).ToListAsync();

            foreach (var item in subComments)
            {
                await BindSubComments(item);
                comment.comments.Add(item);
            }
        }

        public async Task<List<NewsViewModel>> GetRelatedNewsAsync(int number, List<string> tagIdList, string newsId)
        {
            var newsList = new List<NewsViewModel>();
            int randomRow;
            int recentRandomRow = 0;
            tagIdList.Insert(0, newsId);
            string whereExpression = "NewsId!=@0 and (";
            for (int i = 0; i < tagIdList.Count() - 1; i++)
                whereExpression = whereExpression + @"TagId==@" + (i + 1) + (i + 1 != tagIdList.Count - 1 ? " or " : ")");

            int newsCount = (from n in _context.News.Where(n => n.IsPublish == true && n.PublishDateTime <= DateTime.Now)
                             join t in _context.NewsTags.Where(whereExpression, tagIdList.ToArray())
                             on n.NewsId equals t.NewsId
                             select n).Count();


            for (int i = 0; i < number && i < newsCount; i++)
            {
                randomRow = CustomMethods.RandomNumber(1, newsCount + 1);
                while (recentRandomRow == randomRow)
                    randomRow = CustomMethods.RandomNumber(1, newsCount + 1);

                var news = await (from n in _context.News.Where(n => n.IsPublish == true && n.PublishDateTime <= DateTime.Now).Include(c => c.Comments).Include(l => l.Likes).Include(l => l.Visits)
                                  join t in _context.NewsTags.Where(whereExpression, tagIdList.ToArray())
                                  on n.NewsId equals t.NewsId
                                  select new NewsViewModel
                                  {
                                      Title = n.Title,
                                      Url = n.Url,
                                      NewsId = n.NewsId,
                                      ImageName = n.ImageName,
                                      PublishDateTime = n.PublishDateTime,
                                      NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                      NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                      NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                      NumberOfComment = n.Comments.Count(),
                                  }).Skip(randomRow - 1).Take(1).FirstOrDefaultAsync();

                newsList.Add(news);
                recentRandomRow = randomRow;
            }

            return newsList;
        }
        public int GetPublishedNewsCount()
        {
            return _context.News.Where(c => c.IsPublish == true).Count();
        }

        public async Task<List<NewsInCategoriesAndTagsViewModel>> GetNewsInCategoryAsync(string categoryId, int pageIndex, int pageSize)
        {
            string NameOfCategories = "";
            List<NewsInCategoriesAndTagsViewModel> newsViewModel = new List<NewsInCategoriesAndTagsViewModel>();

            var allNews = await (from n in ((from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(u => u.User).Include(c => c.Comments).Include(c => c.NewsCategories)
                                             where (n.IsPublish == true && n.PublishDateTime <= DateTime.Now && n.NewsCategories.Select(c => c.CategoryId).Contains(categoryId))
                                             select (new
                                             {
                                                 n.NewsId,
                                                 n.Title,
                                                 n.Abstract,
                                                 ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                                 n.Url,
                                                 n.ImageName,
                                                 NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                 NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                                 NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                                 NumberOfComments = n.Comments.Where(c => c.IsConfirm == true).Count(),
                                                 AuthorName = n.User.FirstName + " " + n.User.LastName,
                                                 n.PublishDateTime,
                                             })).Skip(pageIndex * pageSize).Take(pageSize))
                                 join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
                                 from bct in bc.DefaultIfEmpty()
                                 join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                                 from cog in cg.DefaultIfEmpty()
                                 select (new NewsInCategoriesAndTagsViewModel
                                 {
                                     NewsId = n.NewsId,
                                     Title = n.Title,
                                     Abstract = n.Abstract,
                                     ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                     Url = n.Url,
                                     ImageName = n.ImageName,
                                     NumberOfVisit = n.NumberOfVisit,
                                     NumberOfLike = n.NumberOfLike,
                                     NumberOfDisLike = n.NumberOfDisLike,
                                     NumberOfComments = n.NumberOfComments,
                                     NameOfCategories = cog != null ? cog.CategoryName : "",
                                     AuthorName = n.AuthorName,
                                     PublishDateTime = n.PublishDateTime,
                                 })).AsNoTracking().ToListAsync();

            var newsGroup = allNews.GroupBy(g => g.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g });
            foreach (var item in newsGroup)
            {
                NameOfCategories = "";
                foreach (var a in item.NewsGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                NewsInCategoriesAndTagsViewModel news = new NewsInCategoriesAndTagsViewModel()
                {
                    NewsId = item.NewsId,
                    Title = item.NewsGroup.First().Title,
                    ShortTitle = item.NewsGroup.First().ShortTitle,
                    Abstract = item.NewsGroup.First().Abstract,
                    Url = item.NewsGroup.First().Url,
                    NumberOfVisit = item.NewsGroup.First().NumberOfVisit,
                    NumberOfDisLike = item.NewsGroup.First().NumberOfDisLike,
                    NumberOfLike = item.NewsGroup.First().NumberOfLike,
                    NameOfCategories = NameOfCategories,
                    ImageName = item.NewsGroup.First().ImageName,
                    AuthorName = item.NewsGroup.First().AuthorName,
                    NumberOfComments = item.NewsGroup.First().NumberOfComments,
                    PersianPublishDate = item.NewsGroup.First().PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                    PersianPublishTime = item.NewsGroup.First().PublishDateTime.ConvertMiladiToShamsi("HH:mm:ss"),
                };
                newsViewModel.Add(news);
            }
            return newsViewModel;
        }

        public async Task<List<NewsInCategoriesAndTagsViewModel>> GetNewsInTagAsync(string TagId, int pageIndex, int pageSize)
        {
            string NameOfCategories = "";
            List<NewsInCategoriesAndTagsViewModel> newsViewModel = new List<NewsInCategoriesAndTagsViewModel>();

            var allNews = await (from n in ((from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(u => u.User).Include(c => c.Comments).Include(c => c.NewsTags)
                                             where (n.IsPublish == true && n.PublishDateTime <= DateTime.Now && n.NewsTags.Select(c => c.TagId).Contains(TagId))
                                             select (new
                                             {
                                                 n.NewsId,
                                                 n.Title,
                                                 n.Abstract,
                                                 ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                                 n.Url,
                                                 n.ImageName,
                                                 NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                                 NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                                 NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                                 NumberOfComments = n.Comments.Where(c => c.IsConfirm == true).Count(),
                                                 AuthorName = n.User.FirstName + " " + n.User.LastName,
                                                 n.PublishDateTime,
                                             })).Skip(pageIndex * pageSize).Take(pageSize))
                                 join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
                                 from bct in bc.DefaultIfEmpty()
                                 join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                                 from cog in cg.DefaultIfEmpty()
                                 select (new NewsInCategoriesAndTagsViewModel
                                 {
                                     NewsId = n.NewsId,
                                     Title = n.Title,
                                     Abstract = n.Abstract,
                                     ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                     Url = n.Url,
                                     ImageName = n.ImageName,
                                     NumberOfVisit = n.NumberOfVisit,
                                     NumberOfLike = n.NumberOfLike,
                                     NumberOfDisLike = n.NumberOfDisLike,
                                     NumberOfComments = n.NumberOfComments,
                                     NameOfCategories = cog != null ? cog.CategoryName : "",
                                     AuthorName = n.AuthorName,
                                     PublishDateTime = n.PublishDateTime,
                                 })).AsNoTracking().ToListAsync();

            var newsGroup = allNews.GroupBy(g => g.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g });
            foreach (var item in newsGroup)
            {
                NameOfCategories = "";
                foreach (var a in item.NewsGroup.Select(a => a.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }

                NewsInCategoriesAndTagsViewModel news = new NewsInCategoriesAndTagsViewModel()
                {
                    NewsId = item.NewsId,
                    Title = item.NewsGroup.First().Title,
                    ShortTitle = item.NewsGroup.First().ShortTitle,
                    Abstract = item.NewsGroup.First().Abstract,
                    Url = item.NewsGroup.First().Url,
                    NumberOfVisit = item.NewsGroup.First().NumberOfVisit,
                    NumberOfDisLike = item.NewsGroup.First().NumberOfDisLike,
                    NumberOfLike = item.NewsGroup.First().NumberOfLike,
                    NameOfCategories = NameOfCategories,
                    ImageName = item.NewsGroup.First().ImageName,
                    AuthorName = item.NewsGroup.First().AuthorName,
                    NumberOfComments = item.NewsGroup.First().NumberOfComments,
                    PersianPublishDate = item.NewsGroup.First().PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd"),
                    PersianPublishTime = item.NewsGroup.First().PublishDateTime.ConvertMiladiToShamsi("HH:mm:ss"),
                };
                newsViewModel.Add(news);
            }
            return newsViewModel;
        }
        public async Task<List<NewsViewModel>> GetUserBookmarksAsync(int userId)
        {
            return await (from u in _context.Users
                          join b in _context.Bookmarks on u.Id equals b.UserId
                          join n in _context.News on b.NewsId equals n.NewsId
                          where (u.Id == userId)
                          select new NewsViewModel { NewsId = n.NewsId, Title = n.Title, PersianPublishDate = n.PublishDateTime.ConvertMiladiToShamsi("dd MMMM yyyy ساعت HH:mm"), Url = n.Url }).ToListAsync();
        }
        public NewsViewModel NumberOfLikeAndDislike(string newsId)
        {
            return (from u in _context.News.Include(l => l.Likes)
                    where (u.NewsId == newsId)
                    select new NewsViewModel { NumberOfLike = u.Likes.Where(l => l.IsLiked == true).Count(), NumberOfDisLike = u.Likes.Where(l => l.IsLiked == false).Count() })
                    .FirstOrDefault();

        }
        public async Task<bool> BookMarkAsync(string newsId, int UserId)
        {
            var bookMark = await _context.Bookmarks.FirstOrDefaultAsync(d => d.NewsId == newsId && d.UserId == UserId);
            if (bookMark == null)
            {
                await _context.Bookmarks.AddAsync(new Bookmark { NewsId = newsId, UserId = UserId });
                await _context.SaveChangesAsync();
                return true;
            }
            _context.Bookmarks.Remove(bookMark);
            await _context.SaveChangesAsync();
            return false;
        }

        public async Task<string> GetWeeklyNewsAsync()
        {
            string content = "";
            int NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
            DateTime StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            DateTime EndMiladiDate = DateTime.Now;
            var news = await _context.News
                .Where(d => d.PublishDateTime >= StartMiladiDate && d.PublishDateTime < EndMiladiDate)
                .Select(d => new NewsViewModel
                {
                    ImageName = d.ImageName,
                    Title = d.Title,
                    ShortTitle = d.Title.Length > 50 ? d.Title.Substring(0, 50) + "..." : d.Title,
                    NewsId = d.NewsId,
                    Url = d.Url,
                }).OrderByDescending(d => d.PublishDateTime).AsNoTracking().ToListAsync();
            var url = _configuration.GetSection("SiteSettings.SiteInfo.Url").Value;
            foreach (var item in news)
            {
                content = content + $"<div style='direction:rtl;font-family:tahoma;text-align:center'> <div class='row align-items-center'> <div class='col-12 col-lg-6'><div class='post-thumbnail'> <img src='{url + "/newsImage/" + item.ImageName}' alt='{item.ImageName}'> </div> </div> <div class='col-12 col-lg-6'> <div class='post-content mt-0'> <h4 style='color:#878484;'>{item.Title}</h4> <p> {item.ShortTitle} <a href='{url}/News/{item.NewsId}/{item.Url}'>[ادامه مطلب]</a> </p> </div> </div> </div> </div><hr/>";
            }
            return content;
        }

        public async Task<List<NewsViewModel>> Search(string searchText ,int offset , int limit)
        {
            List<NewsViewModel> newsViewModel = new List<NewsViewModel>();
            var allNews = await (from n in _context.News.AsNoTracking()
               .Where(e => e.Title.Contains(searchText.Trim()) || e.Abstract.Contains(searchText.Trim()) || e.Description.Contains(searchText.Trim()))
               .Include(e => e.Bookmarks).Include(e => e.Likes).Include(e => e.User).Include(e => e.Visits)
               .Skip(offset).Take(limit)
                             join nc in _context.NewsCategories on n.NewsId equals nc.NewsId into nnc
                             from lnnc in nnc.DefaultIfEmpty()
                             join c in _context.Categories on lnnc.CategoryId equals c.CategoryId into ncc
                             from lncc in ncc.DefaultIfEmpty()
                             select (new NewsViewModel
                             {
                                 NewsId = n.NewsId,
                                 Title = n.Title,
                                 Abstract = n.Abstract,
                                 ShortTitle = n.Title.Length > 50 ? n.Title.Substring(0, 50) + "..." : n.Title,
                                 Url = n.Url,
                                 ImageName = n.ImageName,
                                 Description = n.Description,
                                 NumberOfVisit = n.Visits.Select(v => v.NumberOfVisit).Sum(),
                                 NumberOfLike = n.Likes.Where(l => l.IsLiked == true).Count(),
                                 NumberOfDisLike = n.Likes.Where(l => l.IsLiked == false).Count(),
                                 NumberOfComment = n.Comments.Count(),
                                 AuthorName = n.User.FirstName + " " + n.User.LastName,
                                 IsPublish = n.IsPublish,
                                 NewsType = n.IsInternal == true ? "داخلی" : "خارجی",
                                 PublishDateTime = n.PublishDateTime,
                                 NameOfCategories = lncc != null ? lncc.CategoryName : "",
                             })).ToListAsync();
            var newsGroup = allNews.GroupBy(n => n.NewsId).Select(n => new { NewsId = n.Key, Group = n });
            foreach (var item in newsGroup)
            {
                string NameOfCategories = "";
                foreach (var a in item.Group.Select(s=>s.NameOfCategories).Distinct())
                {
                    if (NameOfCategories == "")
                        NameOfCategories = a;
                    else
                        NameOfCategories = NameOfCategories + " - " + a;
                }
                NewsViewModel news = new NewsViewModel()
                {
                    NewsId = item.NewsId,
                    Title = item.Group.First().Title,
                    ShortTitle = item.Group.First().ShortTitle,
                    Abstract = item.Group.First().Abstract,
                    Url = item.Group.First().Url,
                    NumberOfVisit = item.Group.First().NumberOfVisit,
                    NumberOfDisLike = item.Group.First().NumberOfDisLike,
                    NumberOfLike = item.Group.First().NumberOfLike,
                    ImageName = item.Group.First().ImageName,
                    AuthorName = item.Group.First().AuthorName,
                    NumberOfComment = item.Group.First().NumberOfComment,
                    PersianPublishDate = item.Group.First().PersianPublishDate,
                    PersianPublishTime = item.Group.First().PersianPublishTime,
                    NameOfCategories = item.Group.First().NameOfCategories,
                    SearchText = searchText,
                    PublishDateTime = item.Group.First().PublishDateTime,
                };
                newsViewModel.Add(news);
            }
            return newsViewModel;
        }

        public async Task InsertVisitOfUserAsync(string newsId , string ipAddress)
        {
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
        }
        private List<NewsViewModel> SetCategoryAndTagNames(List<NewsViewModel> news, int offset)
        {
            foreach (var item in news)
            {
                foreach (var category in item.NewsCategories.Select(e => e.Category))
                {
                    if (item.NameOfCategories == null)
                        item.NameOfCategories = category.CategoryName;
                    else
                        item.NameOfCategories = item.NameOfCategories + "-" + category.CategoryName;
                }
                foreach (var tag in item.NewsTags.Select(e => e.Tag))
                {
                    if (item.NameOfTags == null)
                        item.NameOfTags = tag.TagName;
                    else
                        item.NameOfTags = item.NameOfTags + "-" + tag.TagName;
                }
            }
            foreach (var item in news)
                item.Row = ++offset;
            return news;
        }

        private DateTime GetStartDateOfDuration(string duration)
        {
            DateTime StartMiladiDate;

            if (duration == "week")
            {
                int NumOfWeek = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
                StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            }

            else if (duration == "day")
                StartMiladiDate = DateTime.Now.Date + new TimeSpan(0, 0, 0);

            else
            {
                string DayOfMonth = DateTimeExtensions.ConvertMiladiToShamsi(DateTime.Now, "dd").Fa2En();
                StartMiladiDate = DateTime.Now.AddDays((-1) * (int.Parse(DayOfMonth) - 1)).Date + new TimeSpan(0, 0, 0);
            }

            return StartMiladiDate;
        }
    }
}
