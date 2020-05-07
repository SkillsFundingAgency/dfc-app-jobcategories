using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.HostedService
{
    public class DataLoadHostedService : IHostedService
    {
        private const string JobProfileApiName = "JobProfile";
        private const string JobCategoryApiName = "JobCategory";
        private readonly IDataLoadService<ServiceTaxonomyApiClientOptions> dataLoadService;
        private readonly ICosmosRepository<JobProfile> jobProfileRepository;
        private readonly ICosmosRepository<JobCategory> jobCategoryRepository;

        public DataLoadHostedService(IDataLoadService<ServiceTaxonomyApiClientOptions> dataLoadService, ICosmosRepository<JobProfile> jobProfileRepository, ICosmosRepository<JobCategory> jobCategoryRepository)
        {
            this.dataLoadService = dataLoadService;
            this.jobProfileRepository = jobProfileRepository;
            this.jobCategoryRepository = jobCategoryRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var apiJobCategories = await GetJobCategoriesAsync().ConfigureAwait(false);
            var apiJobProfiles = await GetJobProfilesAsync().ConfigureAwait(false);

            var jobCategories = apiJobCategories.Select(x => x.Map());
            var jobProfiles = apiJobProfiles.Select(x => x.Map());

            await RemoveExistingData().ConfigureAwait(false);

            var addJobCategoryTasks = jobCategories.Select(x => jobCategoryRepository.UpsertAsync(x));
            var addJobProfileTasks = jobProfiles.Select(x => jobProfileRepository.UpsertAsync(x));

            await Task.WhenAny(addJobCategoryTasks).ConfigureAwait(false);
            await Task.WhenAny(addJobProfileTasks).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Do nothing
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<JobCategoryApiResponse>> GetJobCategoriesAsync()
        {
            var data = await dataLoadService.GetAllAsync(JobCategoryApiName).ConfigureAwait(false);
            var jobCategories = JsonConvert.DeserializeObject<List<JobCategoryApiResponse>>(data);

            return jobCategories;
        }

        private Task<string> GetJobProfileFromApi(Uri uri)
        {
            var id = uri.Segments.Last().TrimEnd('/');
            return dataLoadService.GetByIdAsync(JobProfileApiName, Guid.Parse(id));
        }

        private async Task RemoveExistingData()
        {
            await jobCategoryRepository.DeleteAllAsync<JobCategory>(nameof(JobCategory).ToLower()).ConfigureAwait(false);
            await jobProfileRepository.DeleteAllAsync<JobProfile>(nameof(JobProfile).ToLower()).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobProfileApiResponse>> GetJobProfilesAsync()
        {
            var data = await dataLoadService.GetAllAsync(JobProfileApiName).ConfigureAwait(false);
            var allApiJobProfiles = JsonConvert.DeserializeObject<IEnumerable<JobProfileApiResponse>>(data);
            return allApiJobProfiles;
        }
    }
}
