using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface IUnitOfWork
    {
        IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class;
        ICategoryRepository CategoryRepository { get; }
        IVideoRepository VideoRepository { get; }
        ITagRepository TagRepository { get; }
        INewsRepository NewsRepository { get; }
        INewsletterRepository NewsletterRepository { get; }
        ICommentRepository CommentRepository { get; }
        NewsDBContext _Context { get; }
        Task Commit();
    }
}
