using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IWritableOptions<SiteSettings> _writableLocations;
        public EmailSender(IWritableOptions<SiteSettings> writableLocations)
        {
            _writableLocations = writableLocations;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var Client = new SmtpClient())
            {
                var Credential = new NetworkCredential
                {
                    UserName = _writableLocations.Value.EmailSetting.Username,
                    Password = _writableLocations.Value.EmailSetting.Password,
                };

                Client.Credentials = Credential;
                Client.Host = _writableLocations.Value.EmailSetting.Host;
                Client.Port = _writableLocations.Value.EmailSetting.Port;
                Client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress(_writableLocations.Value.EmailSetting.Email);
                    emailMessage.Subject = subject;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.Body = message;

                    Client.Send(emailMessage);
                };

                await Task.CompletedTask;
            }
        }
    }
}
