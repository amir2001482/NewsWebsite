using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NewsWebsite.Common;
using NewsWebsite.Common.Api;
using NewsWebsite.IocConfig.Api.Exceptions;
using NewsWebsite.Services.Contracts;
using NewsWebsite.ViewModels.Settings;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewsWebsite.IocConfig
{
    public static class AddCustomAuthenticationExtentions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,SiteSettings siteSettings)
        {
            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
              {
                  var secretkey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.SecretKey);
                  var encryptionkey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.EncrypKey);

                  var validationParameters = new TokenValidationParameters
                  {
                      RequireSignedTokens = true,

                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                      RequireExpirationTime = true,
                      ValidateLifetime = true,

                      ValidateAudience = true, //default : false
                      ValidAudience = siteSettings.JwtSettings.Audience,

                      ValidateIssuer = true, //default : false
                      ValidIssuer = siteSettings.JwtSettings.Issuer,

                      TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                  };

                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  options.TokenValidationParameters = validationParameters;
                  options.Events = new JwtBearerEvents
                  {
                      OnAuthenticationFailed = context =>
                      {
                          if (context.Exception != null)
                              throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);
                          return Task.CompletedTask;
                      },

                      OnTokenValidated = async context =>
                      {
                          var userRepository = context.HttpContext.RequestServices.GetRequiredService<IApplicationUserManager>();

                          var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                          if (claimsIdentity.Claims?.Any() != true)
                              context.Fail("This token has no claims.");

                          var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                          if (!securityStamp.HasValue())
                              context.Fail("This token has no secuirty stamp");

                          var userId = claimsIdentity.GetUserId<string>();
                          var user = await userRepository.GetUserAsync(context.Principal);

                          if (user.SecurityStamp != securityStamp)
                              context.Fail("Token secuirty stamp is not valid.");

                          if (!user.IsActive)
                              context.Fail("User is not active.");
                      },

                      OnChallenge = context =>
                      {
                          if (context.AuthenticateFailure != null)
                              throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                          throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);
                      }

                  };
              });
              //.AddGoogle(options =>
              //{
              //    options.ClientId = "315654760867-d01fsd0fb847vft0fbo6hvbgqghrt5ph.apps.googleusercontent.com";
              //    options.ClientSecret = "F7rY4md1LciG24O_4J_RAPct";
              //})
              //.AddYahoo(options =>
              //{
              //    options.ClientId = "dj0yJmk9aWxnZVZNTGVwVXhWJnM9Y29uc3VtZXJzZWNyZXQmc3Y9MCZ4PWQz";
              //    options.ClientSecret = "9d68b57943e8035cd0771f49d2b54af10797eb4e";
              //});

            return services;
        }
    }
}
