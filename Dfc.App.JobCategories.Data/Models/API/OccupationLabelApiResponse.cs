using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models.API
{
    [ExcludeFromCodeCoverage]
    public class OccupationLabelApiResponse : BaseContentItemModel, IBaseContentItemModel
    {
        [JsonProperty("skos__prefLabel")]
        public string? Title { get; set; }

        public new IList<IBaseContentItemModel> ContentItems { get; set; } = new List<IBaseContentItemModel>();
    }
}
