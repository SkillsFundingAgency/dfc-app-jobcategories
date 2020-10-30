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
            return new OccupationLabel
            {
                ItemId = GetIdFromUrl(resp.Url!),
                Title = resp.Title,
                Uri = resp.Url,
            };
        }

        private static Occupation Map(this OccupationApiResponse resp)
        {
            return new Occupation
            {
                ItemId = GetIdFromUrl(resp.Url!),
                Title = resp.Title,
                Uri = resp.Url,
                OccupationLabels = resp.ContentItems.Where(x => x.ContentType == "OccupationLabel").Select(x => ((OccupationLabelApiResponse)x).Map()),
            };
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
                    Occupation = ((OccupationApiResponse)resp.ContentItems.Where(x => x.ContentType == "occupation").Single()).Map(),
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
                    JobProfiles = resp.ContentItems.Where(x => x.ContentType == "JobProfile").Select(x => ((JobProfileApiResponse)x).Map()),
                    CanonicalName = resp.CanonicalName,
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
