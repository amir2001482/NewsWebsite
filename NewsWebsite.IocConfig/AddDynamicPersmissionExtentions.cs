using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NewsWebsite.Services;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.DynamicAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.IocConfig
{
    public static class AddDynamicPersmissionExtentions
    {
        public static IServiceCollection AddDynamicPersmission(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, DynamicPermissionsAuthorizationHandler>();
            services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>();
            services.AddSingleton<ISecurityTrimmingService, SecurityTrimmingService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ConstantPolicies.DynamicPermission, policy => policy.Requirements.Add(new DynamicPermissionRequirement()));
            });

            return services;
        }
    }
}
