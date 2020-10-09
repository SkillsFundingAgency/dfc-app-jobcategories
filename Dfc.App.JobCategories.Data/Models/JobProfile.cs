using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class JobProfile
    {
        public Guid? ItemId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public Uri? Uri { get; set; }

        public Occupation? Occupation { get; set; }

        public DateTime DateModified { get; set; }
    }
}
