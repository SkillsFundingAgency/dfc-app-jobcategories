using AutoMapper;
using CorrelationId;
using DFC.App.JobCategories.ClientHandlers;
using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Extensions;
using DFC.App.JobCategories.Filters;
using DFC.App.JobCategories.Framework;
using DFC.App.JobCategories.HostedService;
using DFC.App.JobCategories.HttpClientPolicies;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Logger.AppInsights.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobCategories";

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // add the default route
                endpoints.MapControllerRoute("default", "{controller=Health}/{action=Ping}");
            });
            //mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var cosmosDbConnection = configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            services.AddDocumentServices<JobCategory>(cosmosDbConnection, env.IsDevelopment());

            services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"]);
            services.AddSingleton(configuration.GetSection(nameof(ServiceTaxonomyApiClientOptions)).Get<ServiceTaxonomyApiClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddCorrelationId();
            services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
            services.AddSingleton(cosmosDbConnection);
            services.AddTransient<IEventProcessingService, EventProcessingService>();
            services.AddTransient<ICacheReloadService, CacheReloadService>();
            services.AddTransient<CorrelationIdDelegatingHandler>();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddSingleton<IContentCacheService, ContentCacheService>();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();
            var policyRegistry = services.AddPolicyRegistry();

            //Controlled by AppSetting for Integration Tests and Development
            if (bool.Parse(configuration["JobCategories:LoadDataOnStartup"]))
            {
                services.AddHostedServiceTelemetryWrapper();
                services.AddSubscriptionBackgroundService(configuration);
                services.AddHostedService<CacheReloadBackgroundService>();
            }

            services.AddApiServices(configuration, policyRegistry);

            services.AddPolicies(policyRegistry, "content", policyOptions);

            services.AddMvc(config =>
                {
                    config.Filters.Add<LoggingAsynchActionFilter>();
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                })
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}