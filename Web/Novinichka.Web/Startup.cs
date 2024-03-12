using System;
using System.Linq;
using System.Reflection;

using CloudinaryDotNet;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Novinichka.Common;
using Novinichka.Data;
using Novinichka.Data.Common;
using Novinichka.Data.Common.Repositories;
using Novinichka.Data.Models;
using Novinichka.Data.Repositories;
using Novinichka.Data.Seeding;
using Novinichka.Services.CronJobs;
using Novinichka.Services.Data.Implementations;
using Novinichka.Services.Data.Interfaces;
using Novinichka.Services.Mapping;
using Novinichka.Services.Messaging;
using Novinichka.Web.ViewModels;

namespace Novinichka.Web;

public class Startup
{
    private readonly IConfiguration configuration;

    public Startup(IConfiguration configuration)
        => this.configuration = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

        services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        var cloudinaryCredentials = new Account(
            this.configuration["Cloudinary:CloudName"],
            this.configuration["Cloudinary:ApiKey"],
            this.configuration["Cloudinary:ApiSecret"]);

        var cloudinaryUtility = new Cloudinary(cloudinaryCredentials);

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = _ => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        }).AddRazorRuntimeCompilation();

        services.AddRazorPages();
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                this.configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                {
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                }).UseConsole());

        services.AddSingleton(this.configuration);
        services.AddSingleton(cloudinaryUtility);

        services.AddSession();

        // Data repositories
        services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IDbQueryRunner, DbQueryRunner>();

        // Application services
        services.AddTransient<IEmailSender, NullMessageSender>();
        services.AddTransient<INewsService, NewsService>();
        services.AddTransient<ISourcesService, SourcesService>();
        services.AddTransient<ICloudinaryService, CloudinaryService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager)
    {
        AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

        // Seed data on application startup
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var dbContext = serviceScope
                .ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();

            new ApplicationDbContextSeeder()
                .SeedAsync(dbContext, serviceScope.ServiceProvider)
                .GetAwaiter()
                .GetResult();

            this.LoadAllHangfireRecurringJobs(recurringJobManager, dbContext);
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // app.UseCookiePolicy();
        app.UseSession();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        if (env.IsDevelopment())
        {
            _ = app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = 20 });
            app.UseHangfireDashboard("/hangfire"/*, new DashboardOptions { Authorization = new[] { new HangfireAuthFilter() } }*/);
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }

    private void LoadAllHangfireRecurringJobs(IRecurringJobManager recurringJobManager, ApplicationDbContext dbContext)
    {
        var sources = dbContext
            .Sources
            .Where(x => !x.IsDeleted)
            .ToList();

        foreach (var source in sources)
        {
            recurringJobManager.AddOrUpdate<GetLatestNewsCronJob>(
                $"{source.ShortName}",
                x => x.StartWorking(source.TypeName, null),
                "*/5 * * * *");
        }
    }

    private class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext dashboardContext)
            => dashboardContext
                .GetHttpContext()
                .User
                .IsInRole(GlobalConstants.AdministratorRoleName);
    }
}
