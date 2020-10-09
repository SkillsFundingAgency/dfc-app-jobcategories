using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public class CacheReloadService : ICacheReloadService
    {
        private readonly ILogger<CacheReloadService> logger;
        private readonly IContentTypeMappingService contentTypeMappingService;
        private readonly IDocumentService<JobCategory> documentService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;

        public CacheReloadService(ILogger<CacheReloadService> logger, IContentTypeMappingService contentTypeMappingService, IDocumentService<JobCategory> documentService, ICmsApiService cmsApiService, IContentCacheService contentCacheService)
        {
            this.logger = logger;
            this.contentTypeMappingService = contentTypeMappingService;
            this.documentService = documentService;
            this.cmsApiService = cmsApiService;
            this.contentCacheService = contentCacheService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload cache started");

                contentTypeMappingService.AddMapping("JobCategory", typeof(JobCategoryApiResponse));
                contentTypeMappingService.AddMapping("JobProfile", typeof(JobProfileApiResponse));
                contentTypeMappingService.AddMapping("occupation", typeof(OccupationApiResponse));
                contentTypeMappingService.AddMapping("OccupationLabel", typeof(OccupationLabelApiResponse));

                //ignore this relationship to prevent circular dependency causing an infinite loop
                contentTypeMappingService.AddIgnoreRelationship("skos__narrower");

                await RemoveExistingData().ConfigureAwait(false);

                var summaryList = await GetSummaryListAsync().ConfigureAwait(false);

                if (summaryList != null && summaryList.Any())
                {
                    await ProcessSummaryListAsync(summaryList, stoppingToken).ConfigureAwait(false);
                }

                logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in cache reload");
            }
        }

        public async Task<IList<JobCategoriesSummaryItemModel>?> GetSummaryListAsync()
        {
            logger.LogInformation("Get summary list");

            var summaryList = await cmsApiService.GetSummaryAsync<JobCategoriesSummaryItemModel>().ConfigureAwait(false);

            logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        public async Task ProcessSummaryListAsync(IList<JobCategoriesSummaryItemModel> summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Process summary list started");

            foreach (var item in summaryList.OrderByDescending(o => o.Published).ThenBy(o => o.Title))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process summary list cancelled");
                    return;
                }

                await GetAndSaveItemAsync(item, stoppingToken).ConfigureAwait(false);
            }

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(JobCategoriesSummaryItemModel item, CancellationToken stoppingToken)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            try
            {
                logger.LogInformation($"Get details for {item.Title} - {item.Url}");

                var apiDataModel = await cmsApiService.GetItemAsync<JobCategoryApiResponse>(item.Url!).ConfigureAwait(false);

                if (apiDataModel == null)
                {
                    logger.LogWarning($"No details returned from {item.Title} - {item.Url}");
                    return;
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process item get and save cancelled");
                    return;
                }

                var jobCategory = apiDataModel.Map();

                await documentService.UpsertAsync(jobCategory).ConfigureAwait(false);

                contentCacheService.AddOrReplace(jobCategory.Id, jobCategory.JobProfiles.Select(x => Guid.Parse(x.Uri!.Segments.Last().Trim('/'))).ToList(), "JobCategory");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in get and save for {item.Title} - {item.Url}");
            }
        }

        private async Task RemoveExistingData()
        {
            var data = await documentService.GetAllAsync().ConfigureAwait(false);

            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    await documentService.DeleteAsync(item.Id).ConfigureAwait(false);
                }
            }
        }
    }
}
