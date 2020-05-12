using DFC.App.ContactUs.Data.Models;
using DFC.App.JobCategories.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.EventProcessorServices
{
    public class EventMessageService : IEventProcessingService
    {
        private readonly ILogger<EventMessageService> logger;
        private readonly IContentPageService<JobCategory> jobCategoryPageService;
        private readonly IContentPageService<JobProfile> jobProfilePageService;
        private readonly IApiDataService<ServiceTaxonomyApiClientOptions> apiDataService;

        public EventMessageService(ILogger<EventMessageService> logger, IContentPageService<JobCategory> jobCategoryPageService, IContentPageService<JobProfile> jobProfilePageService, IApiDataService<ServiceTaxonomyApiClientOptions> apiDataService)
        {
            this.logger = logger;
            this.jobCategoryPageService = jobCategoryPageService;
            this.jobProfilePageService = jobProfilePageService;
            this.apiDataService = apiDataService;
        }

        public Task<HttpStatusCode> AddOrUpdateAsync(Uri url)
        {
            throw new NotImplementedException();
        }

        public Task<HttpStatusCode> DeleteAsync(Uri url)
        {
            var a = jobProfilePageService.GetAllAsync();
            //Get content type and ID
            //If Category or Profile, straight delete
            //If Occupation or Label, query JPs for containing IDs, remove and update
        }
    }
}
