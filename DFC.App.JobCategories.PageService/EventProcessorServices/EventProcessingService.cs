using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.EventProcessorServices
{
    public class EventProcessingService : IEventProcessingService
    {
        private readonly ILogger<EventProcessingService> logger;
        private readonly IContentPageService<JobCategory> jobCategoryPageService;
        private readonly IContentPageService<JobProfile> jobProfilePageService;
        private readonly IApiDataService<ServiceTaxonomyApiClientOptions> jobProfileApiService;

        public EventProcessingService(ILogger<EventProcessingService> logger, IContentPageService<JobCategory> jobCategoryPageService, IContentPageService<JobProfile> jobProfilePageService, IApiDataService<ServiceTaxonomyApiClientOptions> jobProfileApiService)
        {
            this.logger = logger;
            this.jobCategoryPageService = jobCategoryPageService;
            this.jobProfilePageService = jobProfilePageService;
            this.jobProfileApiService = jobProfileApiService;
        }

        public async Task<HttpStatusCode> AddOrUpdateAsync(Uri url)
        {
            var contentType = url.GetContentItemType().ToLower();
            var id = url.GetContentItemId();
            switch (contentType)
            {
                case "jobcategory":
                    //Get JCs and associated JPS -> Reload
                case "jobprofile":
                    return await RefreshJobProfile(id).ConfigureAwait(false);
                case "occupation":
                    var jobProfilesWithOccupation = await GetJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);
                    var jobProfileUpdateTasks = jobProfilesWithOccupation.Select(x => RefreshJobProfile(x.DocumentId.Value));
                    var results = await Task.WhenAll(jobProfileUpdateTasks).ConfigureAwait(false);
                    //Check all succesful?
                    break;
                case "occupationlabel":
                    var jobProfilesWithOccupationLabels = await GetJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);
                    var jobProfileWithOccupationLabelsUpdateTasks = jobProfilesWithOccupationLabels.Select(x => RefreshJobProfile(x.DocumentId.Value));
                    await Task.WhenAll(jobProfileWithOccupationLabelsUpdateTasks).ConfigureAwait(false);
                    break;
                default:
                    throw new InvalidOperationException($"Received message to delete type: {contentType}, id:{id}. There is no implementation present to process this type.");
            }
        }

        private async Task<HttpStatusCode> RefreshJobProfile(Guid id)
        {
            var newJobProfileApi = await jobProfileApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), id).ConfigureAwait(false);
            var newJobProfile = newJobProfileApi.Map();
            return await jobProfilePageService.UpsertAsync(newJobProfile).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobProfile?>> GetJobProfilesByOccupationIdAsync(Guid occupationId)
        {
            var jobProfilesWithOccupation = await jobProfilePageService.GetByQueryAsync(x => x.Occupation.Uri.ToString().Contains(occupationId.ToString())).ConfigureAwait(false);
            return jobProfilesWithOccupation;
        }

        private async Task<IEnumerable<JobProfile?>> GetJobProfilesByOccupationLabelIdAsync(Guid occupationLabelId)
        {
            var jobProfilesWithOccupationLabels = await jobProfilePageService.GetByQueryAsync(x => x.Occupation.OccupationLabels.Any(x => x.Uri.ToString().Contains(occupationLabelId.ToString()))).ConfigureAwait(false);
            return jobProfilesWithOccupationLabels;
        }

        public async Task<HttpStatusCode> DeleteAsync(Uri url)
        {
            var contentType = url.GetContentItemType().ToLower();
            var id = url.GetContentItemId();
            switch (contentType)
            {
                case "jobcategory":
                    return await jobCategoryPageService.DeleteAsync(id).ConfigureAwait(false);
                case "jobprofile":
                    return await jobProfilePageService.DeleteAsync(id).ConfigureAwait(false);
                case "occupation":
                    //Should never happen due to relationship restrictions in OC
                    var jobProfilesWithOccupation = await GetJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);
                    if (jobProfilesWithOccupation != null)
                    {
                        jobProfilesWithOccupation.Select(c =>
                        {
                            if (c == null)
                            {
                                return c;
                            }

                            c.Occupation = null;
                            return c;
                        }).ToList();
                        var occupationUpdateTasks = jobProfilesWithOccupation.Select(x => jobProfilePageService.UpsertAsync(x));
                        await Task.WhenAll(occupationUpdateTasks).ConfigureAwait(false);
                    }

                    break;
                case "occupationlabel":
                    var jobProfilesWithOccupationLabels = await GetJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);
                    foreach (var jp in jobProfilesWithOccupationLabels)
                    {
                        var newOccLabels = jp!.Occupation!.OccupationLabels.Where(x => !x.Uri.ToString().Contains(id.ToString()));
                        jp.Occupation.OccupationLabels = newOccLabels;
                    }

                    var occupationLabelUpdateTasks = jobProfilesWithOccupationLabels.Select(x => jobProfilePageService.UpsertAsync(x));
                    await Task.WhenAll(occupationLabelUpdateTasks).ConfigureAwait(false);
                    break;
                default:
                    throw new InvalidOperationException($"Received message to delete type: {contentType}, id:{id}. There is no implementation present to process this type.");
            }

            return HttpStatusCode.OK;
        }
    }
}
