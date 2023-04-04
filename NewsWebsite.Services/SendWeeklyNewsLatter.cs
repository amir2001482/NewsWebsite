using Coravel.Invocable;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Services
{
    public class SendWeeklyNewsLatter : IInvocable
    {
        private readonly IEmailSender _emailsender;
        private readonly IUnitOfWork _uw;
        public SendWeeklyNewsLatter(IEmailSender emailSender , IUnitOfWork uw)
        {
            _emailsender = emailSender;
            _uw = uw;
        }
        public async Task Invoke()
        {
            var emailContent = await _uw.NewsRepository.GetWeeklyNewsAsync();
            var users = await _uw.BaseRepository<NewsLetter>().FindByConditionAsync(d => d.IsActive == true);
            if(emailContent != "")
            {
                foreach (var item in users)
                    await _emailsender.SendEmailAsync(item.Email, "خبر نامه هفتگی لاله", emailContent);
            }
        }
    }
}
