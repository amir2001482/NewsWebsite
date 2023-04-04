using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsWebsite.Data;
using NewsWebsite.IocConfig;
using NewsWebsite.IocConfig.Mapping;
using NewsWebsite.Services;
using NewsWebsite.ViewModels.Settings;

namespace NewsWebsite
{
    public class Startup
    {
        private  IConfiguration Configuration { get; }
        private IServiceProvider Services { get; }
        public Startup(IConfiguration configuration , IServiceProvider services)
        {
            Configuration = configuration;
            Services = services;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddDbContext<NewsDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));
            services.AddMvc(opptions =>
            {
                opptions.EnableEndpointRouting = false;
            });
            services.AddCustomServices();
            services.AddControllers().AddNewtonsoftJson();
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.GetTempPath()));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddCustomIdentityServices();
            services.ConfigureWritable<SiteSettings>(Configuration.GetSection("SiteSettings"));
            services.AddScheduler();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCustomIdentityServices();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            var provider = app.ApplicationServices;
            provider.UseScheduler(schedule =>
            {
                schedule.Schedule<SendWeeklyNewsLatter>().Cron("30 20 * * 5");
            }).LogScheduledTaskProgress(Services.GetService<ILogger<IScheduler>>());
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                 name: "default",
                 template: "{controller=Home}/{action=Index}/{id?}"
               );
            });
        }
    }
}
