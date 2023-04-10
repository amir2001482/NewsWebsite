using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Models;
using NewsWebsite.ViewModels.Newsletter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace NewsWebsite.Data.Repositories
{
    public class NewsletterRepository : INewsletterRepository
    {

        private readonly NewsDBContext _context;
        private readonly IMapper _mapper;
        public NewsletterRepository(NewsDBContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<NewsletterViewModel>> GetPaginateNewsletterAsync(PaginateModel model)
        {
            var startAndEndDate = ConvertDateTime.GetStartAndEndDateForSearch(model.searchText);
            List<NewsletterViewModel> newsletter = await _context.Newsletters.Where(c => c.Email.Contains(model.searchText) || (c.RegisterDateTime >= startAndEndDate.StartMiladiDate && c.RegisterDateTime <= startAndEndDate.EndMiladiDate))
                                   .OrderBy(model.orderBy)
                                   .Skip(model.offset).Take(model.limit)
                                   .Select(l => _mapper.Map<NewsletterViewModel>(l)).AsNoTracking().ToListAsync();
            foreach (var item in newsletter)
                item.Row = ++model.offset;
            return newsletter;
        }

    }
}
