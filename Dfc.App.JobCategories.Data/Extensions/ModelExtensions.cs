using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Linq;

namespace DFC.App.JobCategories.Data.Extensions
{
    public static class ModelExtensions
    {
        public static JobProfile Map(this JobProfileApiResponse resp)
        {
            if (resp != null)
            {
                return new JobProfile
                {
                    DocumentId = resp.Uri != null ? Guid.Parse(resp.Uri.Segments.Last().TrimEnd('/')) : Guid.NewGuid(),
                    Description = resp.Description,
                    Title = resp.Title,
                    Uri = resp.Uri,
                    Links = resp.Links,
                    DateModified = DateTime.UtcNow,
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
                    DocumentId = resp.Uri != null ? Guid.Parse(resp.Uri.Segments.Last().TrimEnd('/')) : Guid.NewGuid(),
                    Description = resp.Description,
                    Title = resp.Title,
                    Uri = resp.Uri,
                    Links = resp.Links,

                    //Build a fake URI to get the last segment
                    CanonicalName = new Uri("http://unspecifiedhost" + resp.WebsiteUri).Segments.Last().TrimEnd('/'),
                    DateModified = DateTime.UtcNow,
                };
            }

            throw new InvalidOperationException($"{nameof(resp)} is null");
        }
    }
}
