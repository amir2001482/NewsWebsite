﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Common;
using NewsWebsite.Common.Attributes;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.DynamicAccess;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.Video;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [DisplayName("مدیرت ویدیو ها")]
    public class VideoController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private const string VideoNotFound = "ویدیو درخواستی یافت نشد.";

        public VideoController(IUnitOfWork uw, IMapper mapper, IWebHostEnvironment env)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));

            _env = env;
            _env.CheckArgumentIsNull(nameof(_env));
        }

        [DisplayName("مشاهده")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetVideos(string search, string order, int offset, int limit, string sort)
        {
            var model = new PaginateModel();                                 
            List<VideoViewModel> videos;
            int total = _uw.BaseRepository<Video>().CountEntities();
            if (!search.HasValue())
                search = "";
            if (limit == 0)
                limit = total;
            switch (sort)
            {
                case ("عنوان ویدیو"):
                    if (order == "asc")
                        model.orderBy = "Title";
                    else
                        model.orderBy = "Title Desc";
                    break;
                case ("تاریخ انتشار"):
                    if (order == "asc")
                        model.orderBy = "PublishDateTime";
                    else
                        model.orderBy = "PublishDateTime Desc";
                    break;
                default:
                    model.orderBy = "PublishDateTime";
                    break;
            }
            model.searchText = search;
            model.limit = limit;
            model.offset = offset;
            videos = await _uw.VideoRepository.GetPaginateVideosAsync(model);
            if (search != "")
                total = videos.Count();
            return Json(new { total = total, rows = videos });
        }


        [HttpGet,AjaxOnly()]
        [DisplayName("افزودن یا ویرایش")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderVideo(string videoId)
        {
            var videoViewModel = new VideoViewModel();
            if (videoId.HasValue())
            {
                var video = await _uw.BaseRepository<Video>().FindByIdAsync(videoId);
                if (video != null)
                    videoViewModel = _mapper.Map<VideoViewModel>(video);
                else
                    ModelState.AddModelError(string.Empty,VideoNotFound);
            }
            return PartialView("_RenderVideo", videoViewModel);
        }

        [HttpPost, AjaxOnly()]
        public async Task<IActionResult> CreateOrUpdate(VideoViewModel viewModel)
        {
                if (viewModel.VideoId.HasValue())
                    ModelState.Remove("PosterFile");

                if (ModelState.IsValid)
                {
                    if (viewModel.PosterFile != null)
                        viewModel.Poster = _uw.VideoRepository.CheckVideoFileName(viewModel.PosterFile.FileName);
                    if (viewModel.VideoId.HasValue())
                    {
                        var video = await _uw.BaseRepository<Video>().FindByIdAsync(viewModel.VideoId);
                        if (video != null)
                        {
                        if (viewModel.PosterFile != null)
                        {
                            await viewModel.PosterFile.UploadFileAsync($"{_env.WebRootPath}/posters/{viewModel.Poster}");
                            FileExtensions.DeleteFile($"{_env.WebRootPath}/posters/{video.Poster}");
                        }

                        else
                            viewModel.Poster = video.Poster;
                        _uw.BaseRepository<Video>().Update(_mapper.Map(viewModel, video));
                            await _uw.Commit();
                            TempData["notification"] = EditSuccess;
                        }
                        else
                            ModelState.AddModelError(string.Empty, VideoNotFound);
                    }

                    else
                    {
                        await viewModel.PosterFile.UploadFileAsync($"{_env.WebRootPath}/posters/{viewModel.Poster}");
                        viewModel.VideoId = StringExtensions.GenerateId(10);
                        await _uw.BaseRepository<Video>().CreateAsync(_mapper.Map<Video>(viewModel));
                        await _uw.Commit();
                        TempData["notification"] = InsertSuccess;
                    }
                }

                return PartialView("_RenderVideo", viewModel);
        }


        [HttpGet, AjaxOnly()]
        [DisplayName("حذف")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string videoId)
        {
            if (!videoId.HasValue())
                ModelState.AddModelError(string.Empty, VideoNotFound);
            else
            {
                var video = await _uw.BaseRepository<Video>().FindByIdAsync(videoId);
                if (video == null)
                    ModelState.AddModelError(string.Empty, VideoNotFound);
                else
                    return PartialView("_DeleteConfirmation", video);
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("Delete"), AjaxOnly()]
        public async Task<IActionResult> DeleteConfirmed(Video model)
        {
            if (model.VideoId == null)
                ModelState.AddModelError(string.Empty, VideoNotFound);
            else
            {
                var video = await _uw.BaseRepository<Video>().FindByIdAsync(model.VideoId);
                if (video == null)
                    ModelState.AddModelError(string.Empty, VideoNotFound);
                else
                {
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/posters/{video.Poster}");
                    _uw.BaseRepository<Video>().Delete(video);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", video);
                }
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("DeleteGroup"), AjaxOnly()]
        [DisplayName("حذف گروهی")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ ویدیویی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var video = await _uw.BaseRepository<Video>().FindByIdAsync(item);
                    _uw.BaseRepository<Video>().Delete(video);
                    await _uw.Commit();
                    FileExtensions.DeleteFile($"{_env.WebRootPath}/posters/{video.Poster}");
                }
                TempData["notification"] = "حذف گروهی اطلاعات با موفقیت انجام شد.";
            }

            return PartialView("_DeleteGroup");
        }
    }
}