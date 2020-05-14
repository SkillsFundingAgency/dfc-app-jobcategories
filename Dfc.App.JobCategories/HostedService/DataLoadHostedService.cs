using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.HostedService
{
    public class DataLoadHostedService : IHostedService
    {
        private const string JobProfileApiName = "JobProfile";
        private const string JobCategoryApiName = "JobCategory";

        private readonly IApiExtensions apiExtensions;
        private readonly IContentPageService<JobProfile> jobProfileRepository;
        private readonly IContentPageService<JobCategory> jobCategoryRepository;
        private readonly IJobProfileHelper jobProfileHelper;

        public DataLoadHostedService(IApiExtensions apiExtensions, IContentPageService<JobProfile> jobProfileRepository, IContentPageService<JobCategory> jobCategoryRepository, IJobProfileHelper jobProfileHelper)
        {
            this.apiExtensions = apiExtensions;
            this.jobProfileRepository = jobProfileRepository;
            this.jobCategoryRepository = jobCategoryRepository;
            this.jobProfileHelper = jobProfileHelper;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var apiJobCategories = await apiExtensions.LoadDataAsync<JobCategoryApiResponse>(JobCategoryApiName).ConfigureAwait(false);
            var apiJobProfiles = await apiExtensions.LoadDataAsync<JobProfileApiResponse>(JobProfileApiName).ConfigureAwait(false);

            var jobCategories = apiJobCategories.Select(x => x.Map());
            var jobProfiles = apiJobProfiles.Select(x => x.Map());

            jobProfiles = await jobProfileHelper.AddOccupationAndLabels(jobProfiles).ConfigureAwait(false);

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

        private async Task RemoveExistingData()
        {
            await jobCategoryRepository.DeleteAllAsync().ConfigureAwait(false);
            await jobProfileRepository.DeleteAllAsync().ConfigureAwait(false);
        }
    }
}
