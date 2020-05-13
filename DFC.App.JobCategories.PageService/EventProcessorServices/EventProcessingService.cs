using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
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
        private readonly IApiDataService<ServiceTaxonomyApiClientOptions> apiDataService;
        private readonly IJobProfileHelper jobProfileHelper;

        public EventProcessingService(ILogger<EventProcessingService> logger, IContentPageService<JobCategory> jobCategoryPageService, IContentPageService<JobProfile> jobProfilePageService, IApiDataService<ServiceTaxonomyApiClientOptions> jobProfileApiService, IJobProfileHelper jobProfileHelper)
        {
            this.logger = logger;
            this.jobCategoryPageService = jobCategoryPageService;
            this.jobProfilePageService = jobProfilePageService;
            this.apiDataService = jobProfileApiService;
            this.jobProfileHelper = jobProfileHelper;
        }

        public async Task<HttpStatusCode> AddOrUpdateAsync(Uri url)
        {
            var contentType = url.GetContentItemType().ToLower();
            var id = url.GetContentItemId();

            logger.LogInformation($"Adding/Updating {contentType} Uri {url}");

            switch (contentType)
            {
                case "jobcategory":
                    //Get JCs and associated JPS -> Reload
                    var jobCategory = await apiDataService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), id).ConfigureAwait(false);
                    await jobCategoryPageService.UpsertAsync(jobCategory.Map()).ConfigureAwait(false);
                    var jobCategoryJobProfileUpdateTasks = jobCategory.Links.Where(x => x.LinkValue.Key.ToLower() == "jobprofile").Select(z => RefreshJobProfile(Guid.Parse(z.LinkValue.Value.Href.Segments.Last().TrimEnd('/'))));
                    return ProcessResults(await Task.WhenAll(jobCategoryJobProfileUpdateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
                case "jobprofile":
                    return ProcessResults(await RefreshJobProfile(id).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
                case "occupation":
                    var jobProfilesWithOccupation = await GetJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);
                    var jobProfileUpdateTasks = jobProfilesWithOccupation.Select(async x => await RefreshJobProfile(x.DocumentId.Value));
                    return ProcessResults(await Task.WhenAll(jobProfileUpdateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
                case "occupationlabel":
                    var jobProfilesWithOccupationLabels = await GetJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);
                    var jobProfileWithOccupationLabelsUpdateTasks = jobProfilesWithOccupationLabels.Select(async x => await RefreshJobProfile(x.DocumentId.Value));
                    return ProcessResults(await Task.WhenAll(jobProfileWithOccupationLabelsUpdateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
                default:
                    throw new InvalidOperationException($"Received message to delete type: {contentType}, id:{id}. There is no implementation to process this type.");
            }
        }

        private HttpStatusCode ProcessResults(HttpStatusCode statusCode, Uri url, string actionName)
        {
            return ProcessResults(new HttpStatusCode[] { statusCode }, url, actionName);
        }

        private HttpStatusCode ProcessResults(HttpStatusCode[] httpStatusCode, Uri url, string actionName)
        {
            //Check if all HttpStatusCodes start with 2 for success classificaiton
            if (httpStatusCode.All(x => x.ToString().Substring(0, 1) == "2"))
            {
                logger.LogInformation($"Action: {actionName} with Uri: {url} returned successful status codes: {string.Join(',', httpStatusCode.Distinct())}");
                return HttpStatusCode.OK;
            }
            else
            {
                logger.LogInformation($"Action: {actionName} with Uri: {url} returned unsuccessful status codes: {string.Join(',', httpStatusCode.Distinct())}");
                //Correct status code to return?
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<HttpStatusCode> RefreshJobProfile(Guid id)
        {
            var newJobProfileApi = await apiDataService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), id).ConfigureAwait(false);
            var newJobProfile = newJobProfileApi.Map();
            var constructedJobProfile = await jobProfileHelper.AddOccupationAndLabels(newJobProfile).ConfigureAwait(false);
            return await jobProfilePageService.UpsertAsync(constructedJobProfile).ConfigureAwait(false);
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

            logger.LogInformation($"Deleting {contentType} Uri {url}");

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
