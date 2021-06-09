using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Contexts;
using Models.Repositories;
using Models.Shared;
using Services;
using Services.UserServices;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;

namespace ABTest
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
            services.AddDbContextFactory<PostgresSqlContext>(o=>o.UseNpgsql(Configuration["PostgresSql:ConnectionString"]));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IService<CalculateRollingRetentionXdayContext, IEnumerable<RollingRetentionXDay>>, RollingRetentionXdayService>();
            services.AddTransient<IService<CalculateUserLifetimeContext, IEnumerable<UserLifetime>>, LifetimeService>();
            services.AddControllersWithViews().AddNewtonsoftJson();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddMiniProfiler(options =>
            {
                // ...
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUserRepository db )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiniProfiler();
            db.Migrate(CancellationToken.None).GetAwaiter().GetResult();
   
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMiniProfilerIncludes(new RenderOptions
                {
                    StartHidden = true,
                    PopupToggleKeyboardShortcut = "Ctrl+m",
                });
                endpoints.MapDefaultControllerRoute();
            });
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
            
        }
    }
}
