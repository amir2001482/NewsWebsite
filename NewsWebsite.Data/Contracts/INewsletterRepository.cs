using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Newsletter;
using System;
using System.Collections.Generic;

namespace NewsWebsite.Data.Contracts
{
    public interface INewsletterRepository
    {
        List<NewsletterViewModel> GetPaginateNewsletter(int offset, int limit, string orderByAsc, string searchText);
    }
}
