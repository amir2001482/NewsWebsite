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
        public CommentRepository(NewsDBContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CommentViewModel>> GetPaginateCommentsAsync(PaginateModel model , string newsId , bool? isConfirm)
        {
            try
            {
                var startAndEndDate = ConvertDateTime.GetStartAndEndDateForSearch(model.searchText);
                List<CommentViewModel> comments = await _context.Comments
                    .Where(c => (c.Name.Contains(model.searchText) || c.Email.Contains(model.searchText) || (c.PostageDateTime >= startAndEndDate.StartMiladiDate && c.PostageDateTime <= startAndEndDate.EndMiladiDate)) && (string.IsNullOrEmpty(newsId) == false ? c.NewsId.Contains(newsId) : true) && isConfirm == null ? (c.IsConfirm == true || c.IsConfirm == false) : (isConfirm == true ? c.IsConfirm == true : c.IsConfirm == false))
                    .OrderBy(model.orderBy)
                    .Skip(model.offset).Take(model.limit)
                    .Select(c => _mapper.Map<CommentViewModel>(c)).AsNoTracking().ToListAsync();
                foreach (var item in comments)
                    item.Row = ++model.offset;

                return comments;
            }
            catch (Exception ex)
            {
                return new List<CommentViewModel>();
            }
        }
        public int UnConfiremCommentCount() => _context.Comments.Where(d => d.IsConfirm == false).Count();
    }
}
