using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.Comments;
using NewsWebsite.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using AutoMapper.QueryableExtensions;
using AutoMapper.Configuration;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        public CommentRepository(NewsDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CommentViewModel>> GetPaginateCommentsAsync(PaginateModel model, string newsId, bool? isConfirm)
        {
            var startAndEndDate = model.searchText.GetStartAndEndDateForSearch();
            var convertConfirm = Convert.ToBoolean(isConfirm);
            List<CommentViewModel> comments = await _context.Comments
                .Where(n => (isConfirm == null || (convertConfirm == true ? n.IsConfirm : !n.IsConfirm)) && n.NewsId.Contains(newsId) && (n.Name.Contains(model.searchText) || n.Email.Contains(model.searchText) || (n.PostageDateTime >= startAndEndDate.First() && n.PostageDateTime <= startAndEndDate.Last())))
                .OrderBy(model.orderBy)
                .Skip(model.offset).Take(model.limit)
                .Select(c => _mapper.Map<CommentViewModel>(c)).AsNoTracking().ToListAsync();
            foreach (var item in comments)
                item.Row = ++model.offset;

            return comments;
        }
        public int UnConfiremCommentCount() => _context.Comments.Where(d => d.IsConfirm == false).Count();
    }
}
