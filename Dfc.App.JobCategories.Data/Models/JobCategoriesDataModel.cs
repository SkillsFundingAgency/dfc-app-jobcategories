using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dfc.App.JobCategories.Data.Models
{
    public class JobCategoriesDataModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string PartitionKey => SocLevelTwo;

        public long SequenceNumber { get; set; }

        [Required]
        public string SocLevelTwo { get; set; }

        public CategoryDataModel Data { get; set; }
    }
}