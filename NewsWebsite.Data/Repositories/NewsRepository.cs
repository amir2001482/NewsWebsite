using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public NewsRepository(NewsDBContext context, IMapper mapper , IConfiguration configuration)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));

            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(_configuration));
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

        public async Task<List<NewsViewModel>> GetPaginateNewsAsync(NewsPaginateModel model)
        {
            try
            {
                var newsList = await _context.News
                .Include(e => e.Comments)
                .Include(e => e.Likes)
                .Include(e => e.User)
                .Include(e => e.Visits)
                .Include(e => e.NewsTags).ThenInclude(d => d.Tag)
                .Include(e => e.NewsCategories).ThenInclude(d => d.Category)
                .Where(d => (model.isPublish == null ? true : d.IsPublish == model.isPublish && d.PublishDateTime <= DateTime.Now) && model.isInternal == null ? true : d.IsInternal == model.isInternal)
                .AsNoTracking().ToListAsync();

                var res = _mapper.Map<List<NewsViewModel>>(newsList)
                    .OrderBy(model.orderByAsc)
                    .OrderByDescending(model.orderByDes)
                    .Skip(model.offset).Take(model.limit)
                    .ToList();
                return SetCategoryAndTagNames(res, model.offset);
            }
            catch (Exception ex)
            {
                return new List<NewsViewModel>();
            }

        }

        public async Task<List<NewsViewModel>> MostViewedNewsAsync(int offset, int limit, string duration)
        {
            DateTime StartMiladiDate = GetStartDateOfDuration(duration);
            DateTime EndMiladiDate = DateTime.Now;
            var newsList = await _context.News
               .Include(e => e.Comments)
               .Include(e => e.Likes)
               .Include(e => e.User)
               .Include(e => e.Visits)
               .Include(e => e.NewsTags).ThenInclude(d => d.Tag)
               .Include(e => e.NewsCategories).ThenInclude(d => d.Category)
               .Where(e => e.PublishDateTime >= StartMiladiDate && e.PublishDateTime <= EndMiladiDate)
               .AsNoTracking().ToListAsync();
            var res = SetCategoryAndTagNames(_mapper.Map<List<NewsViewModel>>(newsList), offset);
            return res.OrderByDescending(d => d.NumberOfVisit).Skip(offset).Take(limit).ToList();
        }

        public async Task<List<NewsViewModel>> MostTalkNewsAsync(int offset, int limit, string duration)
        {
            DateTime StartMiladiDate = GetStartDateOfDuration(duration);
            DateTime EndMiladiDate = DateTime.Now;
            var newsList = await _context.News
               .Include(e => e.Comments)
               .Include(e => e.Likes)
               .Include(e => e.User)
               .Include(e => e.Visits)
               .Include(e => e.NewsTags).ThenInclude(d => d.Tag)
               .Include(e => e.NewsCategories).ThenInclude(d => d.Category)
               .Where(e => e.PublishDateTime >= StartMiladiDate && e.PublishDateTime <= EndMiladiDate)
               .AsNoTracking().ToListAsync();
            var res = SetCategoryAndTagNames(_mapper.Map<List<NewsViewModel>>(newsList), offset);
            return res.OrderByDescending(d => d.NumberOfComment).Skip(offset).Take(limit).ToList();
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
        public async Task<NewsViewModel> GetNewsById(string newsId, int userId)
        {
            string NameOfCategories = "";
            var newsGroup = await (from n in _context.News.Include(v => v.Visits).Include(l => l.Likes).Include(u => u.User).Include(c => c.Comments)
                                   join e in _context.NewsCategories on n.NewsId equals e.NewsId into bc
                                   from bct in bc.DefaultIfEmpty()
                                   join c in _context.Categories on bct.CategoryId equals c.CategoryId into cg
                                   from cog in cg.DefaultIfEmpty()
                                   join a in _context.NewsTags on n.NewsId equals a.NewsId into ac
                                   from act in ac.DefaultIfEmpty()
                                   join t in _context.Tags on act.TagId equals t.TagId into tg
                                   from tog in tg.DefaultIfEmpty()
                                   where (n.NewsId == newsId)
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
                                       Bookmarked = n.Bookmarks.Any(d => d.NewsId == newsId && d.UserId == userId),
                                       NewsType = n.IsInternal == true ? "داخلی" : "خارجی",
                                       PublishDateTime = n.PublishDateTime == null ? new DateTime(01, 01, 01) : n.PublishDateTime,
                                       PersianPublishDate = n.PublishDateTime == null ? "-" : n.PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت HH:mm:ss"),
                                   })).GroupBy(b => b.NewsId).Select(g => new { NewsId = g.Key, NewsGroup = g }).AsNoTracking().ToListAsync();


            foreach (var a in newsGroup.First().NewsGroup.Select(a => a.NameOfCategories).Distinct())
            {
                if (NameOfCategories == "")
                    NameOfCategories = a;
                else
                    NameOfCategories = NameOfCategories + " - " + a;
            }

            var news = new NewsViewModel()
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

        public async Task<List<NewsViewModel>> GetRelatedNews(int number, List<string> tagIdList, string newsId)
        {
            var newsList = new List<NewsViewModel>();
            int randomRow;
            int newsCount = _context.News.Include(t => t.NewsTags).Where(n => n.IsPublish == true && n.PublishDateTime <= DateTime.Now && tagIdList.Any(y => n.NewsTags.Select(x => x.TagId).Contains(y)) && n.NewsId != newsId).Count();
            for (int i = 0; i < number && i < newsCount; i++)
            {
                var random = new Random();
                randomRow = random.Next(1, newsCount + 1);
                var news = await _context.News.Include(t => t.NewsTags).Include(c => c.Comments).Include(l => l.Likes).Include(l => l.Visits).Where(n => n.IsPublish == true && n.PublishDateTime <= DateTime.Now && tagIdList.Any(y => n.NewsTags.Select(x => x.TagId).Contains(y)) && n.NewsId != newsId)
                    .Select(n => new NewsViewModel
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
                    })
                    .Skip(randomRow - 1).Take(1).FirstOrDefaultAsync();

                newsList.Add(news);
            }

            return newsList;
        }
        public int GetPublishedNewsCount()
        {
            return _context.News.Where(c => c.IsPublish == true).Count();
        }

        public async Task<List<NewsViewModel>> GetNewsInCategoryOrTag(string categoryId, string TagId)
        {
            var obj = await _context.News.AsNoTracking()
                    .Include(d => d.Comments)
                    .Include(d => d.Likes)
                    .Include(d => d.NewsCategories).ThenInclude(e => e.Category)
                    .Include(d => d.NewsTags).ThenInclude(e => e.Tag)
                    .Include(d => d.User)
                    .Include(d => d.Visits)
                    .Where(d => d.IsPublish == true)
                    .ToListAsync();
            var news = _mapper.Map<List<NewsViewModel>>(obj);
            var res = SetCategoryAndTagNames(news, 0);
            if (categoryId.HasValue())
            {
                res = res.Where(d => d.NewsCategories.Any(e => e.CategoryId == categoryId)).ToList();
            }
            else
            {
                res = res.Where(d => d.NewsTags.Any(e => e.TagId == TagId)).ToList();
            }

            return res;
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
            int NumOfWeek = ConvertDateTime.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
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
                }).OrderByDescending(d=>d.PublishDateTime).AsNoTracking().ToListAsync();
            var url = _configuration.GetSection("SiteSettings.SiteInfo").GetValue<string>("Url");
            foreach (var item in news)
            {
                content = content + $"<div style='direction:rtl;font-family:tahoma;text-align:center'> <div class='row align-items-center'> <div class='col-12 col-lg-6'><div class='post-thumbnail'> <img src='{url + "/newsImage/" + item.ImageName}' alt='{item.ImageName}'> </div> </div> <div class='col-12 col-lg-6'> <div class='post-content mt-0'> <h4 style='color:#878484;'>{item.Title}</h4> <p> {item.ShortTitle} <a href='{url}/News/{item.NewsId}/{item.Url}'>[ادامه مطلب]</a> </p> </div> </div> </div> </div><hr/>";
            }
            return content;
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
                int NumOfWeek = ConvertDateTime.ConvertMiladiToShamsi(DateTime.Now, "dddd").GetNumOfWeek();
                StartMiladiDate = DateTime.Now.AddDays((-1) * NumOfWeek).Date + new TimeSpan(0, 0, 0);
            }

            else if (duration == "day")
                StartMiladiDate = DateTime.Now.Date + new TimeSpan(0, 0, 0);

            else
            {
                string DayOfMonth = ConvertDateTime.ConvertMiladiToShamsi(DateTime.Now, "dd").Fa2En();
                StartMiladiDate = DateTime.Now.AddDays((-1) * (int.Parse(DayOfMonth) - 1)).Date + new TimeSpan(0, 0, 0);
            }

            return StartMiladiDate;
        }
    }
}
