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
        private const string OccupationApiName = "Occupation";
        private const string OccuptionLabelApiName = "OccupationLabel";

        private readonly IApiDataService<ServiceTaxonomyApiClientOptions> dataLoadService;
        private readonly IContentPageService<JobProfile> jobProfileRepository;
        private readonly IContentPageService<JobCategory> jobCategoryRepository;

        public DataLoadHostedService(IApiDataService<ServiceTaxonomyApiClientOptions> dataLoadService, IContentPageService<JobProfile> jobProfileRepository, IContentPageService<JobCategory> jobCategoryRepository)
        {
            this.dataLoadService = dataLoadService;
            this.jobProfileRepository = jobProfileRepository;
            this.jobCategoryRepository = jobCategoryRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var apiJobCategories = await LoadDataAsync<JobCategoryApiResponse>(JobCategoryApiName).ConfigureAwait(false);
            var apiJobProfiles = await LoadDataAsync<JobProfileApiResponse>(JobProfileApiName).ConfigureAwait(false);

            var jobCategories = apiJobCategories.Select(x => x.Map());
            var jobProfiles = apiJobProfiles.Select(x => x.Map());

            jobProfiles = await AddOccupationLabels(jobProfiles).ConfigureAwait(false);

            await RemoveExistingData().ConfigureAwait(false);

            var addJobCategoryTasks = jobCategories.Select(x => jobCategoryRepository.UpsertAsync(x));
            var addJobProfileTasks = jobProfiles.Select(x => jobProfileRepository.UpsertAsync(x));

            await Task.WhenAny(addJobCategoryTasks).ConfigureAwait(false);
            await Task.WhenAny(addJobProfileTasks).ConfigureAwait(false);
        }

        private async Task<IEnumerable<JobProfile>> AddOccupationLabels(IEnumerable<JobProfile> jobProfiles)
        {
            var jpsToReturn = new List<JobProfile>();

            var occupations = await GetOccupations(jobProfiles).ConfigureAwait(false);
            var occupationLabels = await GetOccupationLabels(occupations).ConfigureAwait(false);

            foreach (var jp in jobProfiles)
            {
                var jpOccupationUri = jp.Links.FirstOrDefault(x => x.LinkValue.Key.ToLower() == "occupation").LinkValue.Value.Href;
                var occupation = occupations.FirstOrDefault(x => x.Uri == jpOccupationUri);
                var occupationLinks = occupation.Links.Where(z => z.LinkValue.Key.ToLower() == "occupationlabel" && z.LinkValue.Value.Relationship == "ncs__hasAltLabel").Select(y => y.LinkValue.Value.Href);

                if (occupationLinks == null)
                {
                    continue;
                }

                var jpOccupationlabels = occupationLabels.Where(x => occupationLinks.Contains(x.Uri));

                jp.Occupation = new Occupation(occupation.Title, occupation.Uri, jpOccupationlabels.Select(z => new OccupationLabel(z.Title, z.Uri)));

                jpsToReturn.Add(jp);
            }

            return jpsToReturn;
        }

        private async Task<IEnumerable<OccupationLabelApiResponse>> GetOccupationLabels(IEnumerable<OccupationApiResponse> occupations)
        {
            var allLabels = occupations.Where(y => y != null).SelectMany(x => x.Links.Where(z => z.LinkValue.Key == "occupationlabel" && (z.LinkValue.Value.Relationship == "ncs__hasAltLabel")).Select(y => y.LinkValue.Value.Href.Segments.Last().TrimEnd('/')));

            var tasks = allLabels.Select(x => LoadDataByIdAsync<OccupationLabelApiResponse>(OccuptionLabelApiName, Guid.Parse(x)));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return results.ToList();
        }

        private async Task<IEnumerable<OccupationApiResponse>> GetOccupations(IEnumerable<JobProfile> jobProfiles)
        {
            var tasks = jobProfiles.Select(x => LoadDataByIdAsync<OccupationApiResponse>(OccupationApiName, Guid.Parse(x.Links.FirstOrDefault(x => x.LinkValue.Key == "occupation").LinkValue.Value.Href.Segments.Last().TrimEnd('/').ToString())));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return results.ToList();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //Do nothing
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<T>> LoadDataAsync<T>(string contentType)
            where T : class
        {
            var data = await dataLoadService.GetAllAsync<T>(contentType).ConfigureAwait(false);
            return data;
        }

        private async Task<T> LoadDataByIdAsync<T>(string contentType, Guid id)
           where T : class
        {
            var data = await dataLoadService.GetByIdAsync<T>(contentType, id).ConfigureAwait(false);
            return data;
        }

        private async Task RemoveExistingData()
        {
            await jobCategoryRepository.DeleteAllAsync().ConfigureAwait(false);
            await jobProfileRepository.DeleteAllAsync().ConfigureAwait(false);
        }
    }
}
