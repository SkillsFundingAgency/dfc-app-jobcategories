using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobCategories.Data.Models
{
    public class Occupation
    {
        public Occupation(string title, Uri uri, IEnumerable<OccupationLabel> occupationLabels)
        {
            Title = title;
            Uri = uri;
            OccupationLabels = occupationLabels;
        }

        public string? Title { get; set; }

        public IEnumerable<OccupationLabel>? OccupationLabels { get; set; }

        public Uri? Uri { get; set; }
    }
}
