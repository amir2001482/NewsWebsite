using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.Newsletter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface INewsletterRepository
    {
        Task<List<NewsletterViewModel>> GetPaginateNewsletterAsync(PaginateModel model);
    }
}
