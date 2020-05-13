using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.Helpers
{
    public class JobProfileHelper : IJobProfileHelper
    {
        private const string OccupationApiName = "Occupation";
        private const string OccuptionLabelApiName = "OccupationLabel";
        private readonly IApiExtensions apiExtensions;

        public JobProfileHelper(IApiExtensions apiExtensions)
        {
            this.apiExtensions = apiExtensions;
        }

        public async Task<IEnumerable<JobProfile>> AddOccupationAndLabels(IEnumerable<JobProfile> jobProfiles)
        {
            var jpsToReturn = new List<JobProfile>();

            var occupations = await GetOccupations(jobProfiles).ConfigureAwait(false);

            var occupationLabels = await GetOccupationLabels(occupations).ConfigureAwait(false);

            foreach (var jp in jobProfiles)
            {
                var jpOccupationUri = jp.Links.FirstOrDefault(x => x.LinkValue.Key.ToLower() == "occupation").LinkValue.Value.Href;
                var occupation = occupations.FirstOrDefault(x => x.Uri == jpOccupationUri);

                if (occupation == null)
                {
                    throw new InvalidOperationException($"Occupation for Job Profile {jp.Title} is null");
                }

                var occupationLinks = occupation.Links.Where(z => z.LinkValue.Key.ToLower() == "occupationlabel" && z.LinkValue.Value.Relationship == "ncs__hasAltLabel").Select(y => y.LinkValue.Value.Href);

                if (occupationLinks == null)
                {
                    throw new InvalidOperationException($"No Occupation Labels Job Profile {jp.Title}");
                }

                var jpOccupationlabels = occupationLabels.Where(x => occupationLinks.Contains(x.Uri));

                jp.Occupation = new Occupation(occupation.Title, occupation.Uri, jpOccupationlabels.Select(z => new OccupationLabel(z.Title, z.Uri)));

                jpsToReturn.Add(jp);
            }

            return jpsToReturn;
        }

        public async Task<JobProfile> AddOccupationAndLabels(JobProfile jobProfile)
        {
            var result = await AddOccupationAndLabels(new List<JobProfile> { jobProfile }).ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        private async Task<IEnumerable<OccupationLabelApiResponse>> GetOccupationLabels(IEnumerable<OccupationApiResponse> occupations)
        {
            var allLabels = occupations.Where(y => y != null).SelectMany(x => x.Links.Where(z => z.LinkValue.Key == "occupationlabel" && (z.LinkValue.Value.Relationship == "ncs__hasAltLabel")).Select(y => y.LinkValue.Value.Href.Segments.Last().TrimEnd('/')));

            var tasks = allLabels.Select(x => apiExtensions.LoadDataByIdAsync<OccupationLabelApiResponse>(OccuptionLabelApiName, Guid.Parse(x)));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            if (results.Any(x => x == null))
            {
                throw new InvalidOperationException("Remote API returned null for all Occupation Labels");
            }

            return results.ToList();
        }

        private async Task<IEnumerable<OccupationApiResponse>> GetOccupations(IEnumerable<JobProfile> jobProfiles)
        {
            var tasks = jobProfiles.Select(x => apiExtensions.LoadDataByIdAsync<OccupationApiResponse>(OccupationApiName, Guid.Parse(x.Links.FirstOrDefault(x => x.LinkValue.Key == "occupation").LinkValue.Value.Href.Segments.Last().TrimEnd('/').ToString())));
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            if (results.Any(x => x == null))
            {
                throw new InvalidOperationException("Remote API returned null for all Occupations");
            }

            return results.ToList();
        }
    }
}
