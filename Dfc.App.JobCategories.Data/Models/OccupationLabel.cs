using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class OccupationLabel
    {
        public Guid? ItemId { get; set; }
        public string? Title { get; set; }
        public Uri? Uri { get; set; }
    }
}
