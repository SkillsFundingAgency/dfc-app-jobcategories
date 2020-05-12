using System;

namespace DFC.App.JobCategories.Data.Models
{
    public class OccupationLabel
    {
        public OccupationLabel(string title, Uri uri)
        {
            Title = title;
            Uri = uri;
        }

        public string? Title { get; set; }
        public Uri? Uri { get; set; }
    }
}
