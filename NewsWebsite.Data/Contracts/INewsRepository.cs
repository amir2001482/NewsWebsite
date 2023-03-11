using NewsWebsite.Entities;
using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface INewsRepository
    {
        string CheckNewsFileName(string fileName);
        //Task<List<NewsViewModel>> GetPaginateNewsAsync(int offset, int limit, bool? titleSortAsc, bool? visitSortAsc, bool? likeSortAsc, bool? dislikeSortAsc, bool? publishDateTimeSortAsc, string searchText);
        Task<List<NewsViewModel>> GetPaginateNewsAsync(int offset, int limit, Func<NewsViewModel, object> orderByAsc, Func<NewsViewModel, object> orderByDes, string searchText, bool? isPublish);
        Task<List<NewsViewModel>> MostViewedNewsAsync(int offset, int limit, string duration);
    }
}
