using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Category;
using NewsWebsite.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface ICategoryRepository
    {
        Category FindByCategoryName(string categoryName);
        Task<List<TreeViewCategory>> GetAllCategoriesAsync();
        Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(PaginateModel model);
        bool IsExistCategory(string categoryName, string recentCategoryId = null);
    }
}
