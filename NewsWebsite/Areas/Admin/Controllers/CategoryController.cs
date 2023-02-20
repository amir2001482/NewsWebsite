using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private const string CategoryNotFound = "دسته ی درخواستی یافت نشد.";
        public CategoryController(IUnitOfWork uw)
        {
            _uw = uw;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories(string search, string order, int offset, int limit, string sort)
        {
            List<CategoryViewModel> categories;
            int total = _uw.BaseRepository<Category>().CountEntities();
            if (!search.HasValue())
                search = "";

            if (limit == 0)
                limit = total;

            if (sort == "دسته")
            {
                if (order == "asc")
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, true, null, search);
                else
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, false, null, search);
            }

            else if (sort == "دسته پدر")
            {
                if (order == "asc")
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, null, true, search);
                else
                    categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, null, false, search);
            }

            else
                categories = await _uw.CategoryRepository.GetPaginateCategoriesAsync(offset, limit, null, null, search);

            if (search != "")
                total = categories.Count();
            var res = Json(new { total = total, rows = categories });

            return res;
        }

        [HttpGet]
        public async Task<IActionResult> RenderCategory(string categoryId)
        {
            var categoryViewModel = new CategoryViewModel();
            ViewBag.Categories = _uw.CategoryRepository.GetAllCategories();
            if (categoryId.HasValue())
            {
                var category = await _uw.BaseRepository<Category>().FindByIdAsync(categoryId);
                if (category != null)
                {
                    categoryViewModel.CategoryId = category.CategoryId;
                    categoryViewModel.CategoryName = category.CategoryName;
                    categoryViewModel.ParentCategoryName = category.ParentCategoryId;
                    categoryViewModel.Url = category.Url;
                }
                else
                    ModelState.AddModelError(string.Empty, CategoryNotFound);
            }

            return PartialView("_RenderCategory", categoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string parentCategoryId = null;
                if (viewModel.ParentCategoryName.HasValue())
                {
                    var parentCategory = _uw.CategoryRepository.FindByCategoryName(viewModel.ParentCategoryName);
                    if (parentCategory != null)
                        parentCategoryId = parentCategory.CategoryId;
                    else
                    {
                        Category parent = new Category()
                        {
                            CategoryId = StringExtensions.GenerateId(10),
                            CategoryName = viewModel.CategoryName,
                            Url = viewModel.CategoryName,
                        };
                        await _uw.BaseRepository<Category>().CreateAsync(parent);
                        parentCategoryId = parent.CategoryId;
                    }
                }

                if (viewModel.CategoryId.HasValue())
                {
                    var category = await _uw.BaseRepository<Category>().FindByIdAsync(viewModel.CategoryId);
                    if (category != null)
                    {
                        category.CategoryName = viewModel.CategoryName;
                        category.ParentCategoryId = parentCategoryId;
                        category.Url = viewModel.Url;
                        _uw.BaseRepository<Category>().Update(category);
                        await _uw.Commit();
                        TempData["notification"] = "ویرایش اطلاعات با موفقیت انجام شد.";
                    }
                    else
                        ModelState.AddModelError(string.Empty, CategoryNotFound);
                }

                else
                {
                    Category category = new Category()
                    {
                        CategoryId = StringExtensions.GenerateId(10),
                        CategoryName = viewModel.CategoryName,
                        ParentCategoryId = parentCategoryId,
                        Url = viewModel.Url,
                    };
                    await _uw.BaseRepository<Category>().CreateAsync(category);
                    await _uw.Commit();
                    TempData["notification"] = "درج اطلاعات با موفقیت انجام شد.";
                }
            }

            return PartialView("_RenderCategory", viewModel);
        }
    }
}
