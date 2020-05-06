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
        private readonly IDataLoadService<ServiceTaxonomyApiClientOptions> _dataLoadService;
        private readonly ICosmosRepository<JobProfile> jobProfileRepository;
        private readonly ICosmosRepository<JobCategory> jobCategoryRepository;

        public DataLoadHostedService(IDataLoadService<ServiceTaxonomyApiClientOptions> dataLoadService, ICosmosRepository<JobProfile> jobProfileRepository, ICosmosRepository<JobCategory> jobCategoryRepository)
        {
            _dataLoadService = dataLoadService;
            this.jobProfileRepository = jobProfileRepository;
            this.jobCategoryRepository = jobCategoryRepository;
        }

        private async Task<IEnumerable<JobCategoryApiResponse>> GetJobCategoriesAsync()
        {
            var data = await _dataLoadService.GetAllAsync("JobCategory").ConfigureAwait(false);
            var jobCategories = JsonConvert.DeserializeObject<List<JobCategoryApiResponse>>(data);

            return jobCategories;
        }

        private Task<string> GetJobProfileFromApi(Uri uri)
        {
            var id = uri.Segments.Last().TrimEnd('/');
            return _dataLoadService.GetByIdAsync("JobProfile", Guid.Parse(id));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                var a = ex;
                throw;
            }
        }

        private async Task RemoveExistingData()
        {
            await jobCategoryRepository.DeleteAllAsync(nameof(JobCategory)).ConfigureAwait(false);
            await jobProfileRepository.DeleteAllAsync(nameof(JobProfile)).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobProfileApiResponse>> GetJobProfilesAsync()
        {
            var data = await _dataLoadService.GetAllAsync("JobProfile").ConfigureAwait(false);
            var allJobProfiles = JsonConvert.DeserializeObject<IEnumerable<JobProfileApiResponse>>(data);

            var jobProfileTasks = allJobProfiles.Select(x => GetJobProfileFromApi(x.Uri));
            var results = await Task.WhenAll(jobProfileTasks).ConfigureAwait(false);
            var jobProfiles = results.Select(x => JsonConvert.DeserializeObject<JobProfileApiResponse>(x));

            return jobProfiles;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
