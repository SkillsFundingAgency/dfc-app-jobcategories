using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Enums;
using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FluentNHibernate.Conventions.Inspections;
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
        private readonly IDocumentService<JobCategory> documentService;
        private readonly ICmsApiService cmsApiService;
        private readonly IEventGridService eventGridService;

        public EventProcessingService(ILogger<EventProcessingService> logger, IDocumentService<JobCategory> documentService, ICmsApiService cmsApiService, IEventGridService eventGridService)
        {
            this.logger = logger;
            this.documentService = documentService;
            this.cmsApiService = cmsApiService;
            this.eventGridService = eventGridService;
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
                _ => throw new InvalidOperationException($"Received message to add or update type: {contentType}, id:{id}. There is no implementation to process this type."),
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
            var jobCategoriesWithJobProfilesWithOccupationLabels = await GetJobCategoriesWithJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);

            if (jobCategoriesWithJobProfilesWithOccupationLabels == null || !jobCategoriesWithJobProfilesWithOccupationLabels.Any())
            {
                return HttpStatusCode.NotFound;
            }
            
            foreach (var jc in jobCategoriesWithJobProfilesWithOccupationLabels)
            {
                var jobProfiles = jc.JobProfiles.Where(x => x.Occupation!.OccupationLabels.Any(x => x!.Uri!.ToString().Contains(id.ToString())));

                foreach (var jp in jobProfiles)
                {
                    var newOccLabels = jp!.Occupation!.OccupationLabels.Where(x => !x.Uri!.ToString().Contains(id.ToString()));
                    jp.Occupation.OccupationLabels = newOccLabels;
                }
            }

            var occupationLabelUpdateTasks = jobCategoriesWithJobProfilesWithOccupationLabels.Select(x => documentService.UpsertAsync(x));
            return ProcessResults(await Task.WhenAll(occupationLabelUpdateTasks).ConfigureAwait(false), url, nameof(RemoveOccupationLabel));
        }

        private async Task<HttpStatusCode> RemoveJobCategory(Guid id, Uri url)
        {
            var jobCategory = await documentService.GetByIdAsync(id).ConfigureAwait(false);

            if (jobCategory == null)
            {
                return HttpStatusCode.NotFound;
            }

            var result = await documentService.DeleteAsync(id).ConfigureAwait(false) ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            var processedResults = ProcessResults(result, url, nameof(RemoveJobCategory));

            if (processedResults.IsSuccessStatusCode())
            {
                await eventGridService.SendEventAsync(WebhookCacheOperation.Delete, jobCategory).ConfigureAwait(false);
            }

            return processedResults;
        }

        private async Task<HttpStatusCode> RemoveOccupation(Guid id, Uri url)
        {
            //Should never happen due to relationship restrictions in OC
            var jobCategoriesWithJobProfilesByOccupation = await GetJobCategoriesWithJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);

            if (jobCategoriesWithJobProfilesByOccupation == null || !jobCategoriesWithJobProfilesByOccupation.Any() || jobCategoriesWithJobProfilesByOccupation.Any(x => x == null))
            {
                return HttpStatusCode.NotFound;
            }

            foreach (var jc in jobCategoriesWithJobProfilesByOccupation)
            {
                var jobProfiles = jc.JobProfiles.Where(x => x.Occupation.Uri.ToString().Contains(id.ToString()));

                foreach (var jp in jobProfiles)
                {
                    jp.Occupation = null;
                }
            }

            var occupationUpdateTasks = jobCategoriesWithJobProfilesByOccupation.Select(x => documentService.UpsertAsync(x));
            return ProcessResults(await Task.WhenAll(occupationUpdateTasks).ConfigureAwait(false), url, nameof(RemoveOccupation));
        }

        private async Task<HttpStatusCode> RemoveJobProfile(Guid id, Uri url)
        {
            var statusCodesToReturn = new List<HttpStatusCode>();
            var associatedJobCategories = await GetJobCategoryByJobProfileIdAsync(id).ConfigureAwait(false);

            if (associatedJobCategories == null || !associatedJobCategories.Any())
            {
                return HttpStatusCode.NotFound;
            }

            foreach (var category in associatedJobCategories.ToList())
            {
                category.JobProfiles = category.JobProfiles.Where(x => !x.Uri!.ToString().Contains(id.ToString()));
                statusCodesToReturn.Add(await documentService.UpsertAsync(category).ConfigureAwait(false));
            }

            return ProcessResults(statusCodesToReturn.ToArray(), url, nameof(RemoveJobProfile));
        }

        private async Task<HttpStatusCode> AddOrUpdateOccupationLabelAsync(Uri url, Guid id)
        {
            var jobCategoriesWithJobProfilesByOccupationLabels = await GetJobCategoriesWithJobProfilesByOccupationLabelIdAsync(id).ConfigureAwait(false);
            var updateTasks = new List<Task<HttpStatusCode>>();

            foreach (var jc in jobCategoriesWithJobProfilesByOccupationLabels)
            {
                var jobProfiles = jc.JobProfiles.Where(x => x.Occupation!.OccupationLabels.Any(x => x!.Uri!.ToString().Contains(id.ToString())));
                updateTasks.AddRange(jobProfiles.Select(x => RefreshJobProfile(x.Uri!)));
            }

            return ProcessResults(await Task.WhenAll(updateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> AddOrUpdateOccupationAsync(Uri url, Guid id)
        {
            var jobCategoriesWithJobProfilesWithOccupation = await GetJobCategoriesWithJobProfilesByOccupationIdAsync(id).ConfigureAwait(false);
            var updateTasks = new List<Task<HttpStatusCode>>();

            foreach (var jc in jobCategoriesWithJobProfilesWithOccupation)
            {
                var jobProfiles = jc.JobProfiles.Where(x => x.Occupation!.Uri!.ToString().Contains(id.ToString()));
                updateTasks.AddRange(jobProfiles.Select(x => RefreshJobProfile(x.Uri!)));
            }

            return ProcessResults(await Task.WhenAll(updateTasks).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> AddOrUpdateJobProfileAsync(Uri url, Guid id)
        {
            return ProcessResults(await RefreshJobProfile(url).ConfigureAwait(false), url, nameof(AddOrUpdateAsync));
        }

        private async Task<HttpStatusCode> AddOrUpdateJobCategoryAsync(Uri url, Guid id)
        {
            var apiJobCategory = await cmsApiService.GetItemAsync<JobCategoryApiResponse>(url).ConfigureAwait(false);
            var jobCategory = apiJobCategory.Map();
            var jobCategoryResult = await documentService.UpsertAsync(jobCategory).ConfigureAwait(false);

            if (!jobCategoryResult.IsSuccessStatusCode())
            {
                throw new InvalidOperationException($"{nameof(AddOrUpdateJobCategoryAsync)} Id {id} Uri {url} result was not successful: {jobCategoryResult}");
            }

            var processedResults = ProcessResults(jobCategoryResult, url, nameof(AddOrUpdateAsync));

            if (processedResults.IsSuccessStatusCode())
            {
                await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, jobCategory).ConfigureAwait(false);
            }

            return processedResults;
        }

        private async Task<HttpStatusCode> RefreshJobProfile(Uri url)
        {
            var newJobProfileApi = await cmsApiService.GetItemAsync<JobProfileApiResponse>(url).ConfigureAwait(false);
            var newJobProfile = newJobProfileApi.Map();

            var jobCategoriesForJobProfile = await documentService.GetAsync(x => x.JobProfiles.Any(jp => jp.Uri == url)).ConfigureAwait(false);

            foreach (var jc in jobCategoriesForJobProfile)
            {
                jc.JobProfiles = jc.JobProfiles.Select(x => x.Uri == url ? newJobProfile : x);
            }

            var results = await Task.WhenAll(jobCategoriesForJobProfile.Select(x => documentService.UpsertAsync(x))).ConfigureAwait(false);

            return results.All(x => x == HttpStatusCode.OK)
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError;
        }

        private async Task<IEnumerable<JobCategory?>> GetJobCategoryByJobProfileIdAsync(Guid jobProfileId)
        {
            //Can't use extension for ID here as Cosmos client can't compile expression
            return await documentService.GetAsync(x => x.JobProfiles.Any(jp => jp.Uri.ToString().Contains(jobProfileId.ToString()))).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobCategory?>> GetJobCategoriesWithJobProfilesByOccupationIdAsync(Guid occupationId)
        {
            return await documentService.GetAsync(x => x!.JobProfiles.Any(jp => jp.Occupation!.Uri!.ToString().Contains(occupationId.ToString()))).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobCategory?>> GetJobCategoriesWithJobProfilesByOccupationLabelIdAsync(Guid occupationLabelId)
        {
            return await documentService.GetAsync(x => x!.JobProfiles.Any(jp => jp.Occupation!.OccupationLabels.Any(x => x!.Uri!.ToString().Contains(occupationLabelId.ToString())))).ConfigureAwait(false);
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
                if (httpStatusCode.Length > 1 && httpStatusCode.Distinct().Count() > 1)
                {
                    return HttpStatusCode.InternalServerError;
                }

                return httpStatusCode.FirstOrDefault();
            }
        }
    }
}
