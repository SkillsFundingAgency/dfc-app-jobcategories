using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobCategories.Data.Extensions
{
    public static class ModelExtensions
    {
        public static JobProfile Map(this JobProfileApiResponse resp)
        {
            return new JobProfile
            {
                Description = resp.Description,
                Title = resp.Title,
                Uri = resp.Uri,
                Links = resp.Links,
            };
        }

        public static JobCategory Map(this JobCategoryApiResponse resp)
        {
            return new JobCategory
            {
                Description = resp.Description,
                Title = resp.Title,
                Uri = resp.Uri,
                Links = resp.Links,
                WebsiteUri = resp.WebsiteUri,
            };
        }
    }
}
