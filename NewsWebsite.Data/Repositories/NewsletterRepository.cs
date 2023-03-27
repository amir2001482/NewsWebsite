using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.Newsletter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {

        private readonly NewsDBContext _context;
        public NewsletterRepository(NewsDBContext context)
        {
            _context = context;
        }


        public List<NewsletterViewModel> GetPaginateNewsletter(int offset, int limit, Func<NewsletterViewModel, Object> orderByAscFunc, Func<NewsletterViewModel, Object> orderByDescFunc, string searchText)
        {
            List<NewsletterViewModel> newsletter = _context.Newsletters.Where(c => c.Email.Contains(searchText) || c.RegisterDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss").Contains(searchText))
                                   .Select(l => new NewsletterViewModel { Email = l.Email,IsActive=l.IsActive, PersianRegisterDateTime = l.RegisterDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss") })
                                   .OrderBy(orderByAscFunc).OrderByDescending(orderByDescFunc)
                                   .Skip(offset).Take(limit).ToList();

            foreach (var item in newsletter)
                item.Row = ++offset;

            return newsletter;
        }

    }
}
