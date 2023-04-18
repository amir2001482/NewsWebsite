using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public CategoryList(IUnitOfWork uw , IMemoryCache cache)   
        {
            _uw = uw;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cachEntry =await _cache.GetOrCreate("categories", item =>
            {
               item.AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(365);
               return Task.FromResult(_uw.CategoryRepository.GetAllCategoriesAsync().Result);
           });
            return View(cachEntry);
        }
    }
}
