using System;
using System.Collections.Generic;

namespace DFC.App.JobCategories.Data.Models
{
    public class Occupation
    {
        public Guid? ItemId { get; set; }

        public string? Title { get; set; }

        public IEnumerable<OccupationLabel>? OccupationLabels { get; set; }

        public Uri? Uri { get; set; }
    }
}
