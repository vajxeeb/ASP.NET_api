using Library.Core.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Models.DataContext;
using Services;
using Services.DataService;
using Services.Securites;
using System;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region CORS policy

            services.AddCors(options =>
            {
                // default policy
                options.AddDefaultPolicy(builder => builder
                                            .AllowAnyOrigin()
                                            .AllowAnyMethod()
                                            .AllowAnyHeader()
                                        );
                // named policy
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost").AllowAnyHeader().AllowAnyMethod());
            });

            #endregion

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddMvc();

            #region DbContext

            services.AddDbContext<EfDbContext>(opts => opts.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), options => options.MigrationsAssembly("WebApi")));

            #endregion

            #region data protection service
        
            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromMinutes(2));

            #endregion

            // TODO : New Add
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterJwtBearer(services);

            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            #region Application Service Dependency
           
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IUserSellerService, UserSellerService>();
            services.AddScoped<IMobileVersionService, MobileVersionService>();
            services.AddScoped<IOnlineService, OnlineService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<ICancelBillService, CancelBillService>();
            services.AddScoped<INumberStatusService, NumberStatusService>();
            services.AddScoped<IPeriodService, PeriodService>();
            services.AddScoped<IHistorySalePeriodService, HistorySalePeriodService>();

            #endregion
        }
        private void RegisterJwtBearer(IServiceCollection services)
        {
            DependencyContainer.RegisterJwtBearer(services);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            app.UseHttpsRedirection(); 
            app.UseRouting();

            app.UseCors();
            app.UseCors("CorsPolicy");

            // Linux with Nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
