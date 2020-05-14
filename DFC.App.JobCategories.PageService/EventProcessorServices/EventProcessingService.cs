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

            return contentType switch
            {
                "jobcategory" => await AddOrUpdateJobCategoryAsync(url, id).ConfigureAwait(false),
                "jobprofile" => await AddOrUpdateJobProfileAsync(url, id).ConfigureAwait(false),
                "occupation" => await AddOrUpdateOccupationAsync(url, id).ConfigureAwait(false),
                "occupationlabel" => await AddOrUpdateOccupationLabelAsync(url, id).ConfigureAwait(false),
                _ => throw new InvalidOperationException($"Received message to delete type: {contentType}, id:{id}. There is no implementation to process this type."),
            };
        }

        public async Task<HttpStatusCode> DeleteAsync(Uri url)
        {
            var contentType = url.GetContentItemType().ToLower();
            var id = url.GetContentItemId();

            logger.LogInformation($"Deleting {contentType} Uri {url}");

            return contentType switch
            {
                "jobcategory" => await RemoveJobCategory(id, url).ConfigureAwait(false),
                "jobprofile" => await RemoveJobProfile(id, url).ConfigureAwait(false),
                "occupation" => await RemoveOccupation(id, url).ConfigureAwait(false),
                "occupationlabel" => await RemoveOccupationLabel(id, url).ConfigureAwait(false),
                _ => throw new InvalidOperationException($"Received message to delete type: {contentType}, id:{id}. There is no implementation present to process this type."),
            };
        }

        private async Task<HttpStatusCode> RemoveOccupationLabel(Guid id, Uri url)
        {
            var jobProfilesWithOccupationLabels = await GetJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);
            foreach (var jp in jobProfilesWithOccupationLabels)
            {
                var newOccLabels = jp!.Occupation!.OccupationLabels.Where(x => !x.Uri!.ToString().Contains(id.ToString()));
                jp.Occupation.OccupationLabels = newOccLabels;
            }

            var occupationLabelUpdateTasks = jobProfilesWithOccupationLabels.Select(x => jobProfilePageService.UpsertAsync(x));
            return ProcessResults(await Task.WhenAll(occupationLabelUpdateTasks).ConfigureAwait(false), url, nameof(RemoveOccupationLabel));
        }

        private async Task<HttpStatusCode> RemoveJobCategory(Guid id, Uri url)
        {
            return ProcessResults(await jobCategoryPageService.DeleteAsync(id).ConfigureAwait(false), url, nameof(RemoveJobCategory));
        }

        private async Task<HttpStatusCode> RemoveOccupation(Guid id, Uri url)
        {
            //Should never happen due to relationship restrictions in OC
            var jobProfilesWithOccupation = await GetJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);

            if (jobProfilesWithOccupation == null || !jobProfilesWithOccupation.Any() || jobProfilesWithOccupation.Any(x => x == null))
            {
                throw new InvalidOperationException($"{nameof(RemoveOccupation)} Id {id} Uri {url} returned null/no Job Profiles for given Occupation");
            }

            jobProfilesWithOccupation.Select(c =>
            {
                if (c == null)
                {
                    return c;
                }

                c.Occupation = null;
                return c;
            });

            var occupationUpdateTasks = jobProfilesWithOccupation.Select(x => jobProfilePageService.UpsertAsync(x));
            return ProcessResults(await Task.WhenAll(occupationUpdateTasks).ConfigureAwait(false), url, nameof(RemoveOccupation));
        }

        private async Task<HttpStatusCode> RemoveJobProfile(Guid id, Uri url)
        {
            var statusCodesToReturn = new List<HttpStatusCode>();

            var associatedJobCategories = await GetJobCategoryByJobProfileIdAsync(id).ConfigureAwait(false);

            foreach (var category in associatedJobCategories.ToList())
            {
                var categoryLinks = category!.Links.ToList();

                //Job profile can only exist in a category link once
                var linkToRemove = categoryLinks.FirstOrDefault(x => x.LinkValue.Key == "jobprofile" && x.LinkValue.Value.GetId<Guid>() == id);

                if (linkToRemove == null)
                {
                    throw new InvalidOperationException($"{nameof(RemoveJobProfile)} Id {id} Uri {url} Job Profile not found in Job Category links");
                }

                categoryLinks.Remove(linkToRemove);
                category.Links = categoryLinks;
                statusCodesToReturn.Add(await jobCategoryPageService.UpsertAsync(category).ConfigureAwait(false));
            }

            statusCodesToReturn.Add(await jobProfilePageService.DeleteAsync(id).ConfigureAwait(false));

            return ProcessResults(statusCodesToReturn.ToArray(), url, nameof(RemoveJobProfile));
        }

        private async Task<HttpStatusCode> AddOrUpdateOccupationLabelAsync(Uri url, Guid id)
        {
            var jobProfilesWithOccupationLabels = await GetJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);
            var jobProfileWithOccupationLabelsUpdateTasks = jobProfilesWithOccupationLabels.Select(x => RefreshJobProfile(x!.DocumentId!.Value));
            return ProcessResults(await Task.WhenAll(jobProfileWithOccupationLabelsUpdateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> AddOrUpdateOccupationAsync(Uri url, Guid id)
        {
            var jobProfilesWithOccupation = await GetJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);
            var jobProfileUpdateTasks = jobProfilesWithOccupation.Select(x => RefreshJobProfile(x!.DocumentId!.Value));
            return ProcessResults(await Task.WhenAll(jobProfileUpdateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> AddOrUpdateJobProfileAsync(Uri url, Guid id)
        {
            return ProcessResults(await RefreshJobProfile(id).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> AddOrUpdateJobCategoryAsync(Uri url, Guid id)
        {
            var jobCategory = await apiDataService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), id).ConfigureAwait(false);
            var jobCategoryResult = await jobCategoryPageService.UpsertAsync(jobCategory.Map()).ConfigureAwait(false);

            if (!jobCategoryResult.IsSuccessStatusCode())
            {
                throw new InvalidOperationException($"{nameof(AddOrUpdateJobCategoryAsync)} Id {id} Uri {url} result was not successful: {jobCategoryResult}");
            }

            var jobCategoryJobProfileUpdateTasks = jobCategory.Links.Where(x => x.LinkValue.Key.ToLower() == "jobprofile").Select(z => RefreshJobProfile(z.LinkValue.Value.GetId<Guid>()));
            return ProcessResults(await Task.WhenAll(jobCategoryJobProfileUpdateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> RefreshJobProfile(Guid id)
        {
            var newJobProfileApi = await apiDataService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), id).ConfigureAwait(false);
            var newJobProfile = newJobProfileApi.Map();
            var constructedJobProfile = await jobProfileHelper.AddOccupationAndLabels(newJobProfile).ConfigureAwait(false);
            return await jobProfilePageService.UpsertAsync(constructedJobProfile).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobCategory?>> GetJobCategoryByJobProfileIdAsync(Guid jobProfileId)
        {
            //Can't use extension for ID here as Cosmos client can't compile expression
            var jobCategoriesWithJobProfile = await jobCategoryPageService.GetByQueryAsync(x => x.Links.Any(z => z.LinkValue.Key == "jobprofile" && z.LinkValue.Value.Href!.ToString().Contains(jobProfileId.ToString()))).ConfigureAwait(false);

            if (jobCategoriesWithJobProfile == null || !jobCategoriesWithJobProfile.Any() || jobCategoriesWithJobProfile.Any(x => x == null))
            {
                throw new InvalidOperationException($"{nameof(GetJobCategoryByJobProfileIdAsync)} returned null response for Job Categories with Job Profile");
            }

            return jobCategoriesWithJobProfile;
        }

        private async Task<IEnumerable<JobProfile?>> GetJobProfilesByOccupationIdAsync(Guid occupationId)
        {
            var jobProfilesWithOccupation = await jobProfilePageService.GetByQueryAsync(x => x!.Occupation!.Uri!.ToString().Contains(occupationId.ToString())).ConfigureAwait(false);

            if (jobProfilesWithOccupation == null)
            {
                throw new InvalidOperationException($"{nameof(GetJobProfilesByOccupationIdAsync)} returned null Job Profiles for Occupation {occupationId}");
            }

            return jobProfilesWithOccupation;
        }

        private async Task<IEnumerable<JobProfile?>> GetJobProfilesByOccupationLabelIdAsync(Guid occupationLabelId)
        {
            var jobProfilesWithOccupationLabels = await jobProfilePageService.GetByQueryAsync(x => x!.Occupation!.OccupationLabels.Any(x => x!.Uri!.ToString().Contains(occupationLabelId.ToString()))).ConfigureAwait(false);

            if (jobProfilesWithOccupationLabels == null)
            {
                throw new InvalidOperationException($"{nameof(GetJobProfilesByOccupationLabelIdAsync)} returned null Job Profiles for Occupation Labels {occupationLabelId}");
            }

            return jobProfilesWithOccupationLabels;
        }

        private HttpStatusCode ProcessResults(HttpStatusCode statusCode, Uri url, string actionName)
        {
            return ProcessResults(new HttpStatusCode[] { statusCode }, url, actionName);
        }

        private HttpStatusCode ProcessResults(HttpStatusCode[] httpStatusCode, Uri url, string actionName)
        {
            if (httpStatusCode.All(x => x.IsSuccessStatusCode()))
            {
                logger.LogInformation($"Action: {actionName} with Uri: {url} returned successful status codes: {string.Join(',', httpStatusCode.Distinct())}");
                return HttpStatusCode.OK;
            }
            else
            {
                logger.LogInformation($"Action: {actionName} with Uri: {url} returned unsuccessful status codes: {string.Join(',', httpStatusCode.Distinct())}");

                //Correct status code to return?
                if (httpStatusCode.Length > 1)
                {
                    return HttpStatusCode.InternalServerError;
                }

                return httpStatusCode.FirstOrDefault();
            }
        }
    }
}
