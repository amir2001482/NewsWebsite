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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsWebsite.Data;
using NewsWebsite.IocConfig;
using NewsWebsite.IocConfig.Mapping;
using NewsWebsite.Services;
using NewsWebsite.ViewModels.DynamicAccess;
using NewsWebsite.ViewModels.Settings;

namespace NewsWebsite
{
    public class Startup
    {
        private  IConfiguration Configuration { get; }
        private IServiceProvider Services { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.GetTempPath()));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddCustomIdentityServices();
            services.ConfigureWritable<SiteSettings>(Configuration.GetSection("SiteSettings"));
            services.AddScheduler();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(ConstantPolicies.DynamicPermission, policy => policy.Requirements.Add(new DynamicPermissionRequirement()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                schedule.Schedule<SendWeeklyNewsLatter>().Cron("30 20 * * 5"); // UTC time
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}"
               );
            });
        }
    }
}
