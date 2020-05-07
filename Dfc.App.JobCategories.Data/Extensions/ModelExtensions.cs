﻿using DFC.App.JobCategories.Data.Models;
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
                    Description = resp.Description,
                    Title = resp.Title,
                    Uri = resp.Uri,
                    Links = resp.Links,
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
                    Description = resp.Description,
                    Title = resp.Title,
                    Uri = resp.Uri,
                    Links = resp.Links,

                    //Build a fake URI to get the last segment
                    CanonicalName = new Uri("http://unspecifiedhost" + resp.WebsiteUri).Segments.Last().TrimEnd('/'),
                };
            }

            throw new InvalidOperationException($"{nameof(resp)} is null");
        }
    }
}