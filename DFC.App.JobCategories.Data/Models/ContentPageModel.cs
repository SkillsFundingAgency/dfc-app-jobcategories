using DFC.App.JobCategories.Data.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModel : IDataModel
    {
        [Guid]
        [JsonProperty(PropertyName = "id")]
        public Guid? DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string? Etag { get; set; }

        [Required]
        [LowerCase]
        [UrlPath]
        public string? CanonicalName { get; set; }

        public string? PartitionKey { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public string? BreadcrumbTitle { get; set; }

        [Display(Name = "Include In SiteMap")]
        public bool? IncludeInSitemap { get; set; }

        [UrlPath]
        [LowerCase]
        public IList<string>? AlternativeNames { get; set; }

        public MetaTagsModel MetaTags { get; set; } = new MetaTagsModel();

        public string? Content { get; set; }

        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }

        public Uri? Uri { get; set; }

        public DateTime DateModified { get; set; }
        public string TraceId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
