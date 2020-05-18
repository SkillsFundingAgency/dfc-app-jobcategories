using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class JobProfile : IDataModel
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Uri? Uri { get; set; }

        public IEnumerable<Link>? Links { get; set; }

        public bool? IncludeInSitemap => false;

        public Guid? DocumentId { get; set; }

        public string? Etag { get; set; }

        public string? PartitionKey => "jobprofile";

        public string? CanonicalName => Title != null ? Title.ToLower().Replace(" ", "-", StringComparison.CurrentCultureIgnoreCase) : string.Empty;

        public Occupation? Occupation { get; set; }

        public DateTime DateModified { get; set; }
    }
}
