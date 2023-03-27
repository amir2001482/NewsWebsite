using NewsWebsite.ViewModels.Comments;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Contracts
{
    public interface ICommentRepository
    {
        List<CommentViewModel> GetPaginateComments(int offset, int limit, Func<CommentViewModel, Object> orderByAscFunc, Func<CommentViewModel, Object> orderByDescFunc, string searchText);
    }
}
