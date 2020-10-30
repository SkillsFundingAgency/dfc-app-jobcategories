using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class Occupation
    {
        public Guid? ItemId { get; set; }

        public string? Title { get; set; }

        public IEnumerable<OccupationLabel>? OccupationLabels { get; set; }

        public Uri? Uri { get; set; }
    }
}
