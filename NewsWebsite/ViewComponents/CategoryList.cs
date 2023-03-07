using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.ViewComponents
{
    public class CategoryList : ViewComponent
    {
        private readonly IUnitOfWork _uw;

        public CategoryList(IUnitOfWork uw)
        {
            _uw = uw;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _uw.CategoryRepository.GetAllCategoriesAsync();
            return View(categories);
        }
    }
}
