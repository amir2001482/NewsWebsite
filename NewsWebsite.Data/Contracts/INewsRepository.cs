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
        Task<List<NewsViewModel>> GetPaginateNewsAsync(NewsPaginateModel model);
        Task<List<NewsViewModel>> MostViewedNewsAsync(int offset, int limit, string duration);
        Task<List<NewsViewModel>> MostTalkNewsAsync(int offset, int limit, string duration);
        Task<List<NewsViewModel>> MostPopularNewsAsync(int offset, int limit);

    }
}
