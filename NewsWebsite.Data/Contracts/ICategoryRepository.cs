using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface ICategoryRepository
    {
        Category FindByCategoryName(string categoryName);
        List<TreeViewCategory> GetAllCategories();
        Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, bool? categoryNameSortAsc, bool? parentCategoryNameSortAsc, string searchText);
    }
}
