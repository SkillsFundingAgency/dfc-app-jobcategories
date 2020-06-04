using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.HostedService
{
    public class DataLoadHostedService : IHostedService
    {
        private const string JobProfileApiName = "jobprofile";
        private const string JobCategoryApiName = "jobcategory";

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
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();

            configuration.InstrumentationKey = "af3a60c6-4b76-4cb7-bc6f-b1b4b042bd29";
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            var telemetryClient = new TelemetryClient(configuration);

            var activity = new Activity("DataLoadHostedService").Start();

            try
            {
                var module = new DependencyTrackingTelemetryModule();
                module.Initialize(configuration);

                using (module)
                {
                    telemetryClient.StartOperation<DependencyTelemetry>($"DataLoadHostedService: {nameof(StartAsync)}");
                    telemetryClient.TrackTrace("Importing Job Categories Data");

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
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                activity.Stop();
            }

            // before exit, flush the remaining data
            telemetryClient.Flush();

            // flush is not blocking when not using InMemoryChannel so wait a bit. There is an active issue regarding the need for `Sleep`/`Delay`
            // which is tracked here: https://github.com/microsoft/ApplicationInsights-dotnet/issues/407
            Task.Delay(10000).Wait();
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
