using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class MetaTagsModel
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Keywords { get; set; }
    }
}
