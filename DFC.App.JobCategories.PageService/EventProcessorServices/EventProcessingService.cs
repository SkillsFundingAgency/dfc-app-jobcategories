using DFC.App.JobCategories.Data.Models;
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

        public EventProcessingService(ILogger<EventProcessingService> logger, IContentPageService<JobCategory> jobCategoryPageService, IContentPageService<JobProfile> jobProfilePageService)
        {
            this.logger = logger;
            this.jobCategoryPageService = jobCategoryPageService;
            this.jobProfilePageService = jobProfilePageService;
        }

        public Task<HttpStatusCode> AddOrUpdateAsync(Uri url)
        {
            throw new NotImplementedException();
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
                    var jobProfilesWithOccupation = await jobProfilePageService.GetByQueryAsync(x => x.Occupation.Uri.ToString().Contains(id.ToString())).ConfigureAwait(false);

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
                    var jobProfilesWithOccupationLabels = await jobProfilePageService.GetByQueryAsync(x => x.Occupation.OccupationLabels.Any(x => x.Uri.ToString().Contains(id.ToString()))).ConfigureAwait(false);
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
