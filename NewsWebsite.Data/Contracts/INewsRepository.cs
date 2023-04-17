using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Home;
using NewsWebsite.ViewModels.Models;
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
        Task<List<NewsViewModel>> GetPaginateNewsAsync(PaginateModel model, bool? isPublish, bool? isInternal);
        Task<List<NewsViewModel>> MostViewedNewsAsync(int offset, int limit, string duration);
        Task<List<NewsViewModel>> MostTalkNewsAsync(int offset, int limit, string duration);
        Task<List<NewsViewModel>> MostPopularNewsAsync(int offset, int limit);
        Task<NewsViewModel> GetNewsByIdAsync(string newsId , int userId);
        Task<List<NewsViewModel>> GetNextAndPreviousNews(DateTime? PublishDateTime);
        Task<List<Comment>> GetNewsCommentsAsync(string newsId);
        Task BindSubComments(Comment comment);
        Task<List<NewsViewModel>> GetRelatedNewsAsync(int number, List<string> tagIdList, string newsId);
        Task<List<NewsViewModel>> Search(string searchText, int offset, int limit);
        Task<List<NewsViewModel>> GetUserBookmarksAsync(int userId);
        Task<List<NewsInCategoriesAndTagsViewModel>> GetNewsInCategoryAsync(string categoryId, int pageIndex, int pageSize);
        Task<List<NewsInCategoriesAndTagsViewModel>> GetNewsInTagAsync(string TagId, int pageIndex, int pageSize);
        Task InsertVisitOfUserAsync(string newsId, string ipAddress);
        //Task<NewsViewModel> LikeOrdisLikeAsync(bool isLike, string newsId, string ip);
        Task<bool> BookMarkAsync(string newsId, int UserId);
        Task<string> GetWeeklyNewsAsync();
        NewsViewModel NumberOfLikeAndDislike(string newsId);
        int CountNews();
        int CountFuturePublishedNews();
        int CountNewsPublishedOrDraft(bool isPublish);
        int CountNewsPublished();
        int GetPublishedNewsCount();

    }
}
