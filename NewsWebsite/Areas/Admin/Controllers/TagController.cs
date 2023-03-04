using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Tag;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class TagController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        private const string TagNotFound = "برچسب یافت نشد.";
        private const string TagDuplicate = "نام برچسب تکراری است.";

        public TagController(IUnitOfWork uw, IMapper mapper)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetTags(string search, string order, int offset, int limit, string sort)
        {
            List<TagViewModel> tags;
            int total = _uw.BaseRepository<Tag>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "برچسب")
            {
                if (order == "asc")
                    tags = await _uw.TagRepository.GetPaginateTagsAsync(offset, limit, true, search);
                else
                    tags = await _uw.TagRepository.GetPaginateTagsAsync(offset, limit, false, search);
            }

            else
                tags = await _uw.TagRepository.GetPaginateTagsAsync(offset, limit,null,search);

            if (search != "")
                total = tags.Count();

            return Json(new { total = total, rows = tags });
        }



        [HttpGet]
        public async Task<IActionResult> RenderTag(string tagId)
        {
            var tagViewModel = new TagViewModel();
            if (tagId.HasValue())
            {
                var tag = await _uw.BaseRepository<Tag>().FindByIdAsync(tagId);
                if (tag != null)
                    tagViewModel = _mapper.Map<TagViewModel>(tag);
                else
                    ModelState.AddModelError(string.Empty, TagNotFound);
            }
            return PartialView("_RenderTag", tagViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(TagViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_uw.TagRepository.IsExistTag(viewModel.TagName, viewModel.TagId))
                    ModelState.AddModelError(string.Empty, TagDuplicate);
                else
                {
                    if (viewModel.TagId.HasValue())
                    {
                        var tag = await _uw.BaseRepository<Tag>().FindByIdAsync(viewModel.TagId);
                        if (tag != null)
                        {
                            _uw.BaseRepository<Tag>().Update(_mapper.Map(viewModel, tag));
                            await _uw.Commit();
                            TempData["notification"] = EditSuccess;
                        }
                        else
                            ModelState.AddModelError(string.Empty, TagNotFound);
                    }

                    else
                    {
                        viewModel.TagId = StringExtensions.GenerateId(10);
                        await _uw.BaseRepository<Tag>().CreateAsync(_mapper.Map<Tag>(viewModel));
                        await _uw.Commit();
                        TempData["notification"] = InsertSuccess;
                    }
                }
            }

            return PartialView("_RenderTag", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string tagId)
        {
            if (!tagId.HasValue())
                ModelState.AddModelError(string.Empty, TagNotFound);
            else
            {
                var tag = await _uw.BaseRepository<Tag>().FindByIdAsync(tagId);
                if (tag == null)
                    ModelState.AddModelError(string.Empty, TagNotFound);
                else
                    return PartialView("_DeleteConfirmation", tag);
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Tag model)
        {
            if (model.TagId == null)
                ModelState.AddModelError(string.Empty, TagNotFound);
            else
            {
                var tag = await _uw.BaseRepository<Tag>().FindByIdAsync(model.TagId);
                if (tag == null)
                    ModelState.AddModelError(string.Empty, TagNotFound);
                else
                {
                    _uw.BaseRepository<Tag>().Delete(tag);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", tag);
                }
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("DeleteGroup")]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ برچسبی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var tag = await _uw.BaseRepository<Tag>().FindByIdAsync(item);
                    _uw.BaseRepository<Tag>().Delete(tag);
                }
                await _uw.Commit();
                TempData["notification"] = "حذف گروهی اطلاعات با موفقیت انجام شد.";
            }

            return PartialView("_DeleteGroup");
        }
    }
}