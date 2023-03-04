﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Common.Attributes;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.News;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class NewsController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IHostingEnvironment _env;
        private const string NewsNotFound = "خبر یافت نشد.";
        private readonly IMapper _mapper;

        public NewsController(IUnitOfWork uw, IMapper mapper, IHostingEnvironment env)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));

            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetNews(string search, string order, int offset, int limit, string sort)
        {
            List<NewsViewModel> news;
            int total = _uw.BaseRepository<News>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "ShortTitle")
            {
                if (order == "asc")
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, true, null, null, null, null, search);
                else
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, false, null, null, null, null, search);
            }

            else if (sort == "بازدید")
            {
                if (order == "asc")
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null, true,null,null,null, search);
                else
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null, false, null, null, null, search);
            }

            else if (sort == "لایک")
            {
                if (order == "asc")
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null,null, true,null,null, search);
                else
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null,null, false,null,null, search);
            }

            else if (sort == "دیس لایک")
            {
                if (order == "asc")
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null,null,null, true,null, search);
                else
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null,null,null, false,null, search);
            }

            else if (sort == "تاریخ انتشار")
            {
                if (order == "asc")
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null, null, null,null,true, search);
                else
                    news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null, null, null,null,false, search);
            }

            else
                news = await _uw.NewsRepository.GetPaginateNewsAsync(offset, limit, null, null,null,null,false, search);

            if (search != "")
                total = news.Count();

            return Json(new { total = total, rows = news });
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrUpdate(string newsId)
        {
            NewsViewModel newsViewModel = new NewsViewModel();
            ViewBag.Tags = _uw._Context.Tags.Select(t => t.TagName).ToList();
            newsViewModel.NewsCategoriesViewModel = new NewsCategoriesViewModel(_uw.CategoryRepository.GetAllCategories(), null);
            if (newsId.HasValue())
            {
                var news = await (from n in _uw._Context.News.Include(c => c.NewsCategories)
                            join w in _uw._Context.NewsTags on n.NewsId equals w.NewsId into bc
                            from bct in bc.DefaultIfEmpty()
                            join t in _uw._Context.Tags on bct.TagId equals t.TagId into cg
                            from cog in cg.DefaultIfEmpty()
                            where (n.NewsId == newsId)
                            select new NewsViewModel
                            {
                                NewsId = n.NewsId,
                                Title = n.Title,
                                Description = n.Description,
                                PublishDateTime = n.PublishDateTime,
                                IsPublish = n.IsPublish,
                                ImageName = n.ImageName,
                                IsInternal = n.IsInternal,
                                NewsCategories = n.NewsCategories,
                                Url = n.Url,
                                NameOfTags = cog!=null? cog.TagName:"",
                            }).ToListAsync();

                if (news != null)
                {
                    newsViewModel = _mapper.Map<NewsViewModel>(news.FirstOrDefault());
                    if (news.FirstOrDefault().PublishDateTime > DateTime.Now)
                    {
                        newsViewModel.FuturePublish = true;
                        newsViewModel.PersianPublishDate = news.FirstOrDefault().PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd");
                        newsViewModel.PersianPublishTime = news.FirstOrDefault().PublishDateTime.Value.TimeOfDay.ToString();
                    }
                    newsViewModel.NewsCategoriesViewModel = new NewsCategoriesViewModel(_uw.CategoryRepository.GetAllCategories(),news.FirstOrDefault().NewsCategories.Select(n=>n.CategoryId).ToArray());
                    newsViewModel.NameOfTags = news.Select(t => t.NameOfTags).ToArray().CombineWith(',');
                }

            }

            return View(newsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(NewsViewModel viewModel,string submitButton)
        {
            ViewBag.Tags = _uw._Context.Tags.Select(t => t.TagName).ToList();
            viewModel.NewsCategoriesViewModel = new NewsCategoriesViewModel(_uw.CategoryRepository.GetAllCategories(), null);
            if (!viewModel.FuturePublish)
            {
                ModelState.Remove("PersianPublishTime");
                ModelState.Remove("PersianPublishDate");
            }
            if(viewModel.NewsId.HasValue())
                ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                if (submitButton != "ذخیره پیش نویس")
                    viewModel.IsPublish = true;

                if (viewModel.ImageFile != null)
                    viewModel.ImageName = $"news-{StringExtensions.GenerateId(10)}.jpg";

                if (viewModel.NewsId.HasValue())
                {
                    string[] newTagsArray;
                    var news = _uw.BaseRepository<News>().FindByConditionAsync(n=>n.NewsId==viewModel.NewsId,null,n => n.NewsCategories,n=>n.NewsTags).Result.FirstOrDefault();
                    if (news == null)
                        ModelState.AddModelError(string.Empty, NewsNotFound);
                    else
                    {
                        if (viewModel.IsPublish && news.IsPublish == false)
                            viewModel.PublishDateTime = DateTime.Now;

                        if (viewModel.IsPublish && news.IsPublish == true)
                        {
                            if (viewModel.PersianPublishDate.HasValue())
                            {
                                var persianTimeArray = viewModel.PersianPublishTime.Split(':');
                                viewModel.PublishDateTime = viewModel.PersianPublishDate.ConvertShamsiToMiladi().Date + new TimeSpan(int.Parse(persianTimeArray[0]), int.Parse(persianTimeArray[1]), 0);
                            }
                            else
                                viewModel.PublishDateTime = news.PublishDateTime;
                        }

                        if (viewModel.ImageFile != null)
                        {
                            viewModel.ImageFile.UploadFileBase64($"{_env.WebRootPath}/newsImage/{viewModel.ImageName}");
                            FileExtensions.DeleteFile($"{_env.WebRootPath}/newsImage/{news.ImageName}");
                        }

                        else
                            viewModel.ImageName = news.ImageName;

                        var newsTags = _uw._Context.NewsTags.Include(t => t.Tag).Where(t => t.NewsId == viewModel.NewsId).Select(t => t.Tag).ToList();
                        if (viewModel.NameOfTags.HasValue())
                        {
                            newTagsArray = viewModel.NameOfTags.Split(',');
                            viewModel.NewsTags = await _uw.TagRepository.InsertNewsTags(viewModel.NameOfTags.Split(','), news.NewsId);
                        }
                        else
                            viewModel.NewsTags = news.NewsTags;

                        if (viewModel.CategoryIds == null)
                            viewModel.NewsCategories = news.NewsCategories;
                        else
                            viewModel.NewsCategories = viewModel.CategoryIds.Select(c => new NewsCategory { CategoryId = c, NewsId = news.NewsId }).ToList();

                        viewModel.UserId = news.UserId;
                        _uw.BaseRepository<News>().Update(_mapper.Map(viewModel, news));
                        await _uw.Commit();
                        ViewBag.Alert = "ذخیره تغییرات با موفقیت انجام شد.";
                        viewModel.NewsCategoriesViewModel.CategoryId = viewModel.CategoryIds;
                    }
                }

                else
                {
                    viewModel.ImageFile.UploadFileBase64($"{_env.WebRootPath}/newsImage/{viewModel.ImageName}");
                    viewModel.NewsId = StringExtensions.GenerateId(10);
                    viewModel.UserId = User.Identity.GetUserId<int>();

                    if (viewModel.IsPublish)
                    {
                        if (!viewModel.PersianPublishDate.HasValue())
                            viewModel.PublishDateTime = DateTime.Now;
                        else
                        {
                            var persianTimeArray = viewModel.PersianPublishTime.Split(':');
                            viewModel.PublishDateTime = viewModel.PersianPublishDate.ConvertShamsiToMiladi().Date + new TimeSpan(int.Parse(persianTimeArray[0]), int.Parse(persianTimeArray[1]), 0);
                        }
                    }

                    if (viewModel.CategoryIds != null)
                        viewModel.NewsCategories=viewModel.CategoryIds.Select(c=>new NewsCategory { CategoryId = c }).ToList();
                    else
                        viewModel.NewsCategories = null;

                    if (viewModel.NameOfTags.HasValue())
                        viewModel.NewsTags = await _uw.TagRepository.InsertNewsTags(viewModel.NameOfTags.Split(","));
                    else
                        viewModel.NewsTags = null;

                    await _uw.BaseRepository<News>().CreateAsync(_mapper.Map<News>(viewModel));
                    await _uw.Commit();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(viewModel);
        }


        [HttpGet, AjaxOnly]
        public async Task<IActionResult> Delete(string newsId)
        {
            if (!newsId.HasValue())
                ModelState.AddModelError(string.Empty, NewsNotFound);
            else
            {
                var news = await _uw.BaseRepository<News>().FindByIdAsync(newsId);
                if (news == null)
                    ModelState.AddModelError(string.Empty, NewsNotFound);
                else
                    return PartialView("_DeleteConfirmation", news);
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("Delete"), AjaxOnly]
        public async Task<IActionResult> DeleteConfirmed(News model)
        {
            if (model.NewsId == null)
                ModelState.AddModelError(string.Empty, NewsNotFound);
            else
            {
                var news = await _uw.BaseRepository<News>().FindByIdAsync(model.NewsId);
                if (news == null)
                    ModelState.AddModelError(string.Empty, NewsNotFound);
                else
                {
                    _uw.BaseRepository<News>().Delete(news);
                    await _uw.Commit();
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/newsImage/{news.ImageName}");
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", news);
                }
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("DeleteGroup"), AjaxOnly]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ خبری برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var news = await _uw.BaseRepository<News>().FindByIdAsync(item);
                    _uw.BaseRepository<News>().Delete(news);
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/newsImage/{news.ImageName}");
                }
                await _uw.Commit();
                TempData["notification"] = "حذف گروهی اطلاعات با موفقیت انجام شد.";
            }

            return PartialView("_DeleteGroup");
        }
    }
}