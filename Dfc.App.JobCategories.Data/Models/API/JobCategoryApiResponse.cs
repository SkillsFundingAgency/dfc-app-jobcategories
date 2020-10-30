using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models.API
{
    [ExcludeFromCodeCoverage]
    public class JobCategoryApiResponse : BaseContentItemModel, IBaseContentItemModel
    {
        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        [JsonProperty("pagelocation_FullUrl")]
        public Uri? WebsiteUri { get; set; }

        [JsonProperty("pagelocation_UrlName")]
        public string? CanonicalName { get; set; }

        public string? Description { get; set; }

        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();
    }
}
