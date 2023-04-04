using Microsoft.Extensions.DependencyInjection;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Data.Repositories;
using NewsWebsite.Data.UnitOfWork;
using NewsWebsite.Services;
using NewsWebsite.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.IocConfig
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddTransient<ICommentRepository, CommentRepository>();
            return services;
        }
    }
}
