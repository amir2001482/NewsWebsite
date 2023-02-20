using Microsoft.Extensions.DependencyInjection;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Data.UnitOfWork;
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
            return services;
        }
    }
}
