using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Newsletter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace NewsWebsite.Data.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {

        private readonly NewsDBContext _context;
        public NewsletterRepository(NewsDBContext context)
        {
            _context = context;
        }


        public List<NewsletterViewModel> GetPaginateNewsletter(int offset, int limit, string orderByAsc, string searchText)
        {
            DateTime? startMiladiDate = Convert.ToDateTime("01/01/01");
            DateTime? endMiladiDate = Convert.ToDateTime("01/01/01");
            var dateTimeResult = searchText.CheckShamsiDate();
            if (dateTimeResult.IsShamsi)
            {
                startMiladiDate = (DateTime)dateTimeResult.MiladiDate;
                if (searchText.Contains(":"))
                    endMiladiDate = startMiladiDate;
                else
                    endMiladiDate = startMiladiDate.Value.Date + new TimeSpan(23, 59, 59);
            }
            List<NewsletterViewModel> newsletter = _context.Newsletters.Where(c => c.Email.Contains(searchText) || (c.RegisterDateTime >= startMiladiDate && c.RegisterDateTime <= endMiladiDate))
                                   .OrderBy(orderByAsc)
                                   .Skip(offset).Take(limit)
                                   .Select(l => new NewsletterViewModel { Email = l.Email, IsActive = l.IsActive, PersianRegisterDateTime = l.RegisterDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss") }).ToList();

            foreach (var item in newsletter)
                item.Row = ++offset;

            return newsletter;
        }

    }
}
