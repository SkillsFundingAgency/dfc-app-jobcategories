using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFC.App.JobCategories.Data.Models
{
    public class JobCategory : IDataModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri Uri { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public bool? IncludeInSitemap => true;
        public Guid? DocumentId { get; set; }
        public string? Etag { get; set; }
        public string? PartitionKey => "jobcategory";
        public string? CanonicalName { get; set; }
    }
}
