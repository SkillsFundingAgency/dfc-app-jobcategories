using DFC.Compui.Cosmos.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class JobCategory : DocumentModel, IDocumentModel, IDataModel
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Uri? Uri { get; set; }

        public IEnumerable<JobProfile>? JobProfiles { get; set; }

        public bool? IncludeInSitemap => true;

        public override string? PartitionKey { get; set; } = "jobcategory";

        public string? CanonicalName { get; set; }

        public DateTime DateModified { get; set; }

        [JsonIgnore]
        public List<Guid>? AllContentItemIds => GetAllContentItemIds();

        private List<Guid>? GetAllContentItemIds()
        {
            return JobProfiles?
                .Select(x => x.ItemId!.Value)
                .Union(JobProfiles?
                    .Select(x => x.Occupation!.ItemId!.Value)
                    .Union(JobProfiles?
                        .SelectMany(x => x.Occupation!.OccupationLabels.Select(c => c.ItemId!.Value))))
                .ToList();
        }
    }
}
