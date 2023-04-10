using NewsWebsite.ViewModels.Comments;
using NewsWebsite.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface ICommentRepository
    {
        Task<List<CommentViewModel>> GetPaginateCommentsAsync(PaginateModel model, string newsId, bool? isConfirm);
        int UnConfiremCommentCount();
    }
}
