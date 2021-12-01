using Library.Core.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Library.Core.IoC
{
    public class DependencyContainer
    {
        public static void RegisterJwtBearer(IServiceCollection services)
        {
            // accepts any access token issued by identity server
            services.AddAuthentication(option =>
            {
                //option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = JwtTokenValidationParameters.GetTokenValidationParameters();
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("TokenExpired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            
            //// adds an authorization policy to make sure the token is for scope 'api1'
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiScope", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("scope", "api1");
            //    });
            //});

            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();
            services.AddHostedService<JwtRefreshTokenCache>();
            services.AddScoped<IRetrieveDataFromTokenService, RetrieveDataFromTokenService>();

        }
    }
}
