﻿using NewsWebsite.Entities;
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
        Task<NewsViewModel> GetNewsById(string newsId , int userId);
        Task<List<NewsViewModel>> GetNextAndPreviousNews(DateTime? PublishDateTime);
        Task<List<Comment>> GetNewsCommentsAsync(string newsId);
        Task BindSubComments(Comment comment);
        Task<List<NewsViewModel>> GetRelatedNews(int number, List<string> tagIdList, string newsId);
        int GetPublishedNewsCount();
        Task<List<NewsViewModel>> GetNewsInCategoryOrTag(string categoryId, string TagId);
        Task<List<NewsViewModel>> GetUserBookmarksAsync(int userId);
        //Task<NewsViewModel> LikeOrdisLikeAsync(bool isLike, string newsId, string ip);
        NewsViewModel NumberOfLikeAndDislike(string newsId);
        Task<bool> BookMarkAsync(string newsId, int UserId);


    }
}
