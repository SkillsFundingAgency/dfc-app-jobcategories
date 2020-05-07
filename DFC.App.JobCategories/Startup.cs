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
using DFC.App.JobCategories.Repository.CosmosDb;
using DFC.Logger.AppInsights.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobCategories";

        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
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
            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
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
            var documentClient  = new DocumentClient(cosmosDbConnection!.EndpointUrl, cosmosDbConnection!.AccessKey);

            services.AddSingleton(configuration.GetSection(nameof(ServiceTaxonomyApiClientOptions)).Get<ServiceTaxonomyApiClientOptions>());
            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddCorrelationId();
            services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
            services.AddSingleton(cosmosDbConnection);
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ICosmosRepository<JobProfile>, CosmosRepository<JobProfile>>();
            services.AddSingleton<ICosmosRepository<JobCategory>, CosmosRepository<JobCategory>>();
            services.AddScoped<IContentPageService, ContentPageService>();
            //services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"]);
            services.AddTransient<CorrelationIdDelegatingHandler>();
            services.AddAutoMapper(typeof(Startup).Assembly);

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>();
            var policyRegistry = services.AddPolicyRegistry();

            //Controlled by AppSetting for Integration Tests and Development
            if (bool.Parse(configuration["JobCategories:LoadDataOnStartup"]))
            {
                services.AddHostedService<DataLoadHostedService>();
            }

            services
               .AddPolicies(policyRegistry, nameof(ServiceTaxonomyApiClientOptions), policyOptions)
               .AddHttpClient<IDataLoadService<ServiceTaxonomyApiClientOptions>, DataLoadService<ServiceTaxonomyApiClientOptions>, ServiceTaxonomyApiClientOptions>(configuration, nameof(ServiceTaxonomyApiClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

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