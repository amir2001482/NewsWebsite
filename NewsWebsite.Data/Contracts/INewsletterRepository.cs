using NewsWebsite.ViewModels.Newsletter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Contracts
{
    public interface INewsletterRepository
    {
        List<NewsletterViewModel> GetPaginateNewsletter(int offset, int limit, Func<NewsletterViewModel, Object> orderByAscFunc, Func<NewsletterViewModel, Object> orderByDescFunc, string searchText);
    }
}
