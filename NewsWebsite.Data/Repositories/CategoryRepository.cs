using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Category;

namespace NewsWebsite.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly NewsDBContext _context;
        public CategoryRepository(NewsDBContext context)
        {
            _context = context;
        }
        public async Task<List<CategoryViewModel>> GetPaginateCategoriesAsync(int offset, int limit, bool? categoryNameSortAsc,bool? parentCategoryNameSortAsc, string searchText)
        {
            List<CategoryViewModel> categories;
            if (categoryNameSortAsc != null)
            {
                categories = await _context.Categories.Include(c => c.category)
                                    .Where(c => c.CategoryName.Contains(searchText.Trim()) || c.category.CategoryName.Contains(searchText.Trim()))
                                    .Select(c => new CategoryViewModel { CategoryId = c.CategoryId, CategoryName = c.CategoryName, Url = c.Url, ParentCategoryName = c.category.CategoryName != null ? c.category.CategoryName : "-" })
                                    .OrderBy(c => (categoryNameSortAsc == true && categoryNameSortAsc != null) ? c.CategoryName : "")
                                    .OrderByDescending(c => (categoryNameSortAsc == false && categoryNameSortAsc != null) ? c.CategoryName : "").Skip(offset).Take(limit).AsNoTracking().ToListAsync();
            }

            else if (parentCategoryNameSortAsc!=null)
            {
                categories = await _context.Categories.Include(c => c.category)
                                   .Where(c => c.CategoryName.Contains(searchText) || c.category.CategoryName.Contains(searchText))
                                   .Select(c => new CategoryViewModel { CategoryId = c.CategoryId, CategoryName = c.CategoryName, Url = c.Url, ParentCategoryName = c.category.CategoryName != null ? c.category.CategoryName : "-" })
                                   .OrderBy(c => (parentCategoryNameSortAsc == true && parentCategoryNameSortAsc != null) ? c.ParentCategoryName : "")
                                   .OrderByDescending(c => (parentCategoryNameSortAsc == false && parentCategoryNameSortAsc != null) ? c.ParentCategoryName : "").Skip(offset).Take(limit).AsNoTracking().ToListAsync();
            }
            else
            {
                categories = await _context.Categories.Include(c => c.category)
                                    .Where(c => c.CategoryName.Contains(searchText) || c.category.CategoryName.Contains(searchText))
                                    .Select(c => new CategoryViewModel {CategoryId= c.CategoryId,CategoryName= c.CategoryName,Url=c.Url,ParentCategoryName=c.category.CategoryName!= null?c.category.CategoryName:"-"})
                                    .Skip(offset).Take(limit).AsNoTracking().ToListAsync();

            }
            foreach (var item in categories)
                item.Row = ++offset;

            return categories;
        }


        public List<TreeViewCategory> GetAllCategories()
        {
            var Categories = (from c in _context.Categories
                              where (c.ParentCategoryId == null)
                              select new TreeViewCategory { id = c.CategoryId, title = c.CategoryName }).ToList();
            foreach (var item in Categories)
            {
                BindSubCategories(item);
            }

            return Categories;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories
                                 where (c.ParentCategoryId == category.id)
                                 select new TreeViewCategory { id = c.CategoryId, title = c.CategoryName }).ToList();
            foreach (var item in SubCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }
        }

        public Category FindByCategoryName(string categoryName)
        {
           return  _context.Categories.Where(c => c.CategoryName == categoryName.TrimStart().TrimEnd()).FirstOrDefault();
        }

    }
}
