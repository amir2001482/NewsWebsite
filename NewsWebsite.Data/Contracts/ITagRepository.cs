using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Tag;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface ITagRepository
    {
        Task<List<TagViewModel>> GetPaginateTagsAsync(int offset, int limit, bool? tagNameSortAsc, string searchText);
        bool IsExistTag(string tagName, string recentTagId = null);
        Task<List<NewsTag>> InsertNewsTags(string[] tags, string newsId = null);
    }
}
