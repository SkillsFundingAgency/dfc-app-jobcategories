using Newtonsoft.Json;
using System;

namespace Dfc.App.JobCategories.Data.Models
{
    public interface IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        string Etag { get; set; }

        string PartitionKey { get; }
    }
}