using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.ViewModels.Dashboard;

namespace NewsWebsite.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IUnitOfWork _uw;
        public DashboardController(IUnitOfWork uw)
        {
            _uw = uw;
        }
        public IActionResult Index()
        {
            ViewBag.News = _uw.NewsRepository.CountNews();
            ViewBag.FuturePublishedNews = _uw.NewsRepository.CountFuturePublishedNews();
            ViewBag.NewsPublished = _uw.NewsRepository.CountNewsPublishedOrDraft(true);
            ViewBag.DraftNews = _uw.NewsRepository.CountNewsPublishedOrDraft(false);

            var month = StringExtensions.GetMonth();
            int numberOfVisit;
            var year = ConvertDateTime.ConvertMiladiToShamsi(DateTime.Now, "yyyy");
            DateTime StartDateTimeMiladi;
            DateTime EndDateTimeMiladi;
            var numberOfVisitList = new List<NumberOfVisitChartViewModel>();

            for (int i = 0; i < month.Length; i++)
            {
                StartDateTimeMiladi = ConvertDateTime.ConvertShamsiToMiladi($"{year}/{i + 1}/01");
                if (i < 11)
                    EndDateTimeMiladi = ConvertDateTime.ConvertShamsiToMiladi($"{year}/{i + 2}/01");
                else
                    EndDateTimeMiladi = ConvertDateTime.ConvertShamsiToMiladi($"{year}/01/01");

                numberOfVisit = _uw._Context.News.Where(n => n.PublishDateTime < EndDateTimeMiladi && StartDateTimeMiladi <= n.PublishDateTime).Include(v => v.Visits).Select(k => k.Visits.Select(v => v.NumberOfVisit).Sum()).Sum();
                numberOfVisitList.Add(new NumberOfVisitChartViewModel { Name = month[i], Value = numberOfVisit });
            }

            ViewBag.NumberOfVisitChart = numberOfVisitList;
            return View();
        }
    }
}