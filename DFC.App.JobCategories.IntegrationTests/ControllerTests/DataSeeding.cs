using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace DFC.App.JobCategories.IntegrationTests.ControllerTests
{
    public static class DataSeeding
    {
        public const string DefaultArticleName = "retail-and-sales";
        public const string AlternativeArticleName = "retail-and-sales";

        public static void SeedDefaultArticles(CustomWebApplicationFactory<DFC.App.JobCategories.Startup> factory)
        {
            const string url = "/pages";
            var contentPageModels = new List<JobCategory>()
            {
                new JobCategory()
                {
                    Id = Guid.Parse("5DDE75FF-8B32-4746-9712-2672E5C540DB"),
                    CanonicalName = "care-worker",
                    Description = "care worker description",
                },
                new JobCategory()
                {
                    Id = Guid.Parse("5DDE75FF-8B31-4746-9712-2672E5C540DB"),
                    CanonicalName = "refuse-worker",
                    Description = "collects refuse",
                },
                new JobCategory()
                {
                    Id = Guid.Parse("5DDE75FF-8B32-4746-9212-2672E5C540DB"),
                    CanonicalName = "aid-worker",
                    Description = "aid worker description",
                },
            };

            var client = factory?.CreateClient();

            client?.DefaultRequestHeaders.Accept.Clear();

            contentPageModels.ForEach(f => client.PostAsync(url, f, new JsonMediaTypeFormatter()).GetAwaiter().GetResult());
        }
    }
}
