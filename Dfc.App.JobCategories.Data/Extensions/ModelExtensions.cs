using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Linq;

namespace DFC.App.JobCategories.Data.Extensions
{
    public static class ModelExtensions
    {
        private static OccupationLabel Map(this OccupationLabelApiResponse resp)
        {
            if (resp != null)
            {
                return new OccupationLabel
                {
                    ItemId = GetIdFromUrl(resp.Url!),
                    Title = resp.Title,
                    Uri = resp.Url,
                };
            }

            throw new InvalidOperationException($"{nameof(resp)} is null");
        }

        private static Occupation Map(this OccupationApiResponse resp)
        {
            if (resp != null)
            {
                return new Occupation
                {
                    ItemId = GetIdFromUrl(resp.Url!),
                    Title = resp.Title,
                    Uri = resp.Url,
                    OccupationLabels = resp.ContentItems.Select(x => ((OccupationLabelApiResponse)x).Map()),
                };
            }

            throw new InvalidOperationException($"{nameof(resp)} is null");
        }

        public static JobProfile Map(this JobProfileApiResponse resp)
        {
            if (resp != null)
            {
                return new JobProfile
                {
                    ItemId = GetIdFromUrl(resp.Url!),
                    Description = resp.Description,
                    Title = resp.Title,
                    Uri = resp.Url,
                    DateModified = DateTime.UtcNow,
                    Occupation = ((OccupationApiResponse)resp.ContentItems.Single()).Map(),
                };
            }

            throw new InvalidOperationException($"{nameof(resp)} is null");
        }

        public static JobCategory Map(this JobCategoryApiResponse resp)
        {
            if (resp != null)
            {
                return new JobCategory
                {
                    Id = resp.Url != null ? GetIdFromUrl(resp.Url) : Guid.NewGuid(),
                    Description = resp.Description,
                    Title = resp.Title,
                    Uri = resp.Url,
                    JobProfiles = resp.ContentItems.Select(x => ((JobProfileApiResponse)x).Map()),

                    //Build a fake URI to get the last segment
                    CanonicalName = new Uri("http://unspecifiedhost" + resp.WebsiteUri).Segments.Last().TrimEnd('/'),
                    DateModified = DateTime.UtcNow,
                };
            }

            throw new InvalidOperationException($"{nameof(resp)} is null");
        }

        private static Guid GetIdFromUrl(Uri url)
        {
            return Guid.Parse(url.Segments.Last().TrimEnd('/'));
        }
    }
}
