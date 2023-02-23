using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NewsWebsite.Services.Contracts;
using NewsWebsite.Services.Identity;

namespace NewsWebsite.IocConfig
{
    public static class IdentityServicesRegistry
    {
        public static void AddCustomIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityOptions();
            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();
            services.AddScoped<ApplicationIdentityErrorDescriber>();
        }

        public static void UseCustomIdentityServices(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.CallDbInitializer();
        }


        private static void CallDbInitializer(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var identityDbInitialize = scope.ServiceProvider.GetService<IIdentityDbInitializer>();
                identityDbInitialize.Initialize();
                identityDbInitialize.SeedData();
            }
        }
    }
}
