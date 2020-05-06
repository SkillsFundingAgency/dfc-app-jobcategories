using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobCategories.Data.Models
{
    public class JobProfile : IDataModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Uri? Uri { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public bool? IncludeInSitemap => false;
        public Guid? DocumentId { get; set; }
        public string? Etag { get; set; }
        public string? PartitionKey => "jobprofile";
        public string? CanonicalName => Title.ToLower().Replace(" ", "-", StringComparison.CurrentCultureIgnoreCase);
    }
}
