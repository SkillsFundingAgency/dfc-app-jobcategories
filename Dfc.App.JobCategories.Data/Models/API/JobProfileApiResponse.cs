using DFC.App.JobCategories.Data.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models.API
{
    [ExcludeFromCodeCoverage]
    public class JobProfileApiResponse
    {
        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        [JsonProperty("Description")]
        public string? Description { get; set; }

        [JsonProperty("uri")]
        public Uri? Uri { get; set; }

        [JsonProperty("_links")]
        [JsonConverter(typeof(DynamicKeyJsonConverter))]
        public IEnumerable<Link>? Links { get; set; }
    }
}
