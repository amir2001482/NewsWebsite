using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using AutoMapper;

namespace NewsWebsite.Data.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        public TagRepository(NewsDBContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<TagViewModel>> GetPaginateTagsAsync(PaginateModel model)
        {
            List<TagViewModel> tags = await _context.Tags
                .Where(c => c.TagName.Contains(model.searchText))
                .OrderBy(model.orderBy)
                .Skip(model.offset).Take(model.limit)
                .Select(c => _mapper.Map<TagViewModel>(c)).AsNoTracking().ToListAsync();          
            foreach (var item in tags)
                item.Row = ++model.offset;
            return tags;
        }
        public bool IsExistTag(string tagName, string recentTagId = null)
        {
            if (!recentTagId.HasValue())
                return _context.Tags.Any(c => c.TagName.Trim().Replace(" ", "") == tagName.Trim().Replace(" ", ""));
            else
            {
                var tag = _context.Tags.Where(c => c.TagName.Trim().Replace(" ", "") == tagName.Trim().Replace(" ", "")).FirstOrDefault();
                if (tag == null)
                    return false;
                else
                {
                    if (tag.TagId != recentTagId)
                        return true;
                    else
                        return false;
                }
            }
        }

        public async Task<List<NewsTag>> InsertNewsTags(string[] tags, string newsId = null)
        {
            string tagId;
            List<NewsTag> newsTags = new List<NewsTag>();
            var allTags = _context.Tags.ToList();
            newsTags.AddRange(allTags.Where(n => tags.Contains(n.TagName)).Select(c => new NewsTag { TagId = c.TagId, NewsId = newsId }).ToList());
            var newTags = tags.Where(n => !allTags.Select(t => t.TagName).Contains(n)).ToList();
            foreach (var item in newTags)
            {
                tagId = StringExtensions.GenerateId(10);
                _context.Tags.Add(new Tag { TagName = item, TagId = tagId });
                newsTags.Add(new NewsTag { TagId = tagId, NewsId = newsId });
            }
            await _context.SaveChangesAsync();
            return newsTags;
        }
        public async Task<Tag> FindByIdAsync(string tagId)
        {
            return await _context.Tags.FirstOrDefaultAsync(d => d.TagId == tagId);
        }
    }
}
