﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewsWebsite.Common;
using NewsWebsite.Common.Attributes;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Category;
using NewsWebsite.ViewModels.DynamicAccess;
using NewsWebsite.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [DisplayName("مدیریت دسته بندی ها")]
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CategoryNotFound = "دسته ی درخواستی یافت نشد.";
        private const string Categoryduplicated = "نام دسته تکراری است.";
        public CategoryController(IUnitOfWork uw , IMapper mapper , IMemoryCache cache)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));
            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
            _cache = cache;
            _cache.CheckArgumentIsNull(nameof(_cache));
        }

        [HttpGet]
        [DisplayName("مشاهده")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories(string search, string order, int offset, int limit, string sort)
        {
            List<CategoryViewModel> categories;
            var model = new PaginateModel();
            int total = _uw.BaseRepository<Category>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;
            switch (sort)
            {
                case ("دسته"):
                    if (order == "asc")
                        model.orderBy = "CategoryInfo.CategoryName";
                    else
                        model.orderBy = "CategoryInfo.CategoryName Desc";
                    break;
                case ("دسته پدر"):
                    if (order == "asc")
                        model.orderBy = "ParentInfo.CategoryName";
                    else
                        model.orderBy = "ParentInfo.CategoryName Desc";
                    break;
                default:
                    model.orderBy = "CategoryInfo.CategoryName";
                    break;
            }
            model.searchText = search;
            model.limit = limit;
            model.offset = offset;
            categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(model);
            if (search != "")
                total = categories.Count();
            var res = Json(new { total = total, rows = categories });

            return res;
        }

        [HttpGet , AjaxOnly()]
        [DisplayName("افزودن و ویرایش")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> RenderCategory(string categoryId)
        {
            var categoryViewModel = new CategoryViewModel();
            ViewBag.Categories = await _uw.CategoryRepository.GetAllCategoriesAsync();
            if (categoryId.HasValue())
            {
                var category = await _uw.BaseRepository<Category>().FindByIdAsync(categoryId);
                await _uw._Context.Entry(category).Reference(c => c.Parent).LoadAsync();
                if (category != null)
                    categoryViewModel = _mapper.Map<CategoryViewModel>(category);
                else
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
            }

            return PartialView("_RenderCategory", categoryViewModel);
        }

        [HttpPost , AjaxOnly()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Url = viewModel.Url.Trim();
               if (_uw.CategoryRepository.IsExistCategory(viewModel.CategoryName, viewModel.CategoryId))
                        ModelState.AddModelError(string.Empty, Categoryduplicated);
                else
                    {
                        _cache.Remove("categories");
                        if (viewModel.ParentCategoryName.HasValue())
                        {
                            var parentCategory = _uw.CategoryRepository.FindByCategoryName(viewModel.ParentCategoryName);
                            if (parentCategory != null)
                                viewModel.ParentCategoryId = parentCategory.CategoryId;
                            else
                            {
                                Category parent = new Category()
                                {
                                    CategoryId = StringExtensions.GenerateId(10),
                                    CategoryName = viewModel.ParentCategoryName,
                                    Url = viewModel.ParentCategoryName,
                                };
                                await _uw.BaseRepository<Category>().CreateAsync(parent);
                                viewModel.ParentCategoryId = parent.CategoryId;
                            }
                        }

                        if (viewModel.CategoryId.HasValue())
                        {
                            var category = await _uw.BaseRepository<Category>().FindByIdAsync(viewModel.CategoryId);
                            if (category != null)
                            {
                                _uw.BaseRepository<Category>().Update(_mapper.Map(viewModel, category));
                                await _uw.Commit();
                                TempData["notification"] = "ویرایش اطلاعات با موفقیت انجام شد.";
                            }
                            else
                                ModelState.AddModelError(string.Empty, CategoryNotFound);
                        }

                        else
                        {
                            viewModel.CategoryId = StringExtensions.GenerateId(10);
                            await _uw.BaseRepository<Category>().CreateAsync(_mapper.Map<Category>(viewModel));
                            await _uw.Commit();
                            TempData["notification"] = "درج اطلاعات با موفقیت انجام شد.";
                        }
                    }
            }
            return PartialView("_RenderCategory", viewModel);
        }
        [HttpGet]
        [DisplayName("حذف")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string categoryId)
        {
            if (!categoryId.HasValue())
                ModelState.AddModelError(string.Empty, CategoryNotFound);
            else
            {
                var category = await _uw.BaseRepository<Category>().FindByIdAsync(categoryId);
                if (category == null)
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
                else
                    return PartialView("_DeleteConfirmation", category);
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("Delete"), AjaxOnly()]
        public async Task<IActionResult> DeleteConfirmed(Category model)
        {
            try
            {
                if (model.CategoryId == null)
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
                else
                {
                    var category = await _uw.BaseRepository<Category>().FindByIdAsync(model.CategoryId);
                    if (category == null)
                        ModelState.AddModelError(string.Empty, CategoryNotFound);
                    else
                    {
                        var childCategory = _uw.BaseRepository<Category>().FindByConditionAsync(c => c.ParentCategoryId == category.CategoryId).Result.ToList();
                        if(childCategory.Count() > 0)
                        {
                            _uw.BaseRepository<Category>().DeleteRange(childCategory);
                            await _uw.Commit();
                        }
                        _uw.BaseRepository<Category>().Delete(category);
                        await _uw.Commit();
                        TempData["notification"] = "حذف اطلاعات با موفقیت انجام شد.";
                        return PartialView("_DeleteConfirmation", category);
                    }
                }
                return PartialView("_DeleteConfirmation");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, ActionName("DeleteGroup") , AjaxOnly()]
        [DisplayName("حذف گروهی")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ دسته بندی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var childCategory = _uw.BaseRepository<Category>().FindByConditionAsync(c => c.ParentCategoryId == item).Result.ToList();
                    if (childCategory.Count() != 0)
                    {
                        _uw.BaseRepository<Category>().DeleteRange(childCategory);
                        await _uw.Commit();
                    }
                    var category = await _uw.BaseRepository<Category>().FindByIdAsync(item);
                    _uw.BaseRepository<Category>().Delete(category);
                    await _uw.Commit();
                }
                TempData["notification"] = "حذف گروهی اطلاعات با موفقیت انجام شد.";
            }

            return PartialView("_DeleteGroup");
        }

    }
}
