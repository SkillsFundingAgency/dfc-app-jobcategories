using AutoMapper;
using DFC.App.JobCategories.Framework;
using Dfc.App.JobCategories.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobCategories.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Dfc.App.JobCategories.MessageFunctionApp
{
    [ExcludeFromCodeCoverage]
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var jobProfileClientOptions = configuration.GetSection("JobCategoriesClientOptions").Get<JobCategoriesClientOptions>();

            builder.AddDependencyInjection();

            builder?.Services.AddSingleton(jobProfileClientOptions);
            builder?.Services.AddAutoMapper(typeof(Startup).Assembly);
            builder?.Services.AddTransient(provider => new HttpClient());
            builder?.Services.AddDFCLogging(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            builder?.Services.AddScoped<ICorrelationIdProvider, InMemoryCorrelationIdProvider>();
            builder?.Services.AddTransient<IMessageProcessor, MessageProcessor>();
            builder?.Services.AddSingleton<IMessagePropertiesService, MessagePropertiesService>();
        }
    }
}