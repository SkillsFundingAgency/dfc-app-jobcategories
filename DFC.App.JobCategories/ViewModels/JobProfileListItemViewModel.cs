using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class JobProfileListItemViewModel
    {
        public JobProfileListItemViewModel(string title, string url, IEnumerable<string>? altTitles, string description)
        {
            Title = title;
            URL = url;
            AltTitles = altTitles != null ? string.Join(", ", altTitles) : null;
            Description = description;
        }

        public string Title { get; }
        public string URL { get; }
        public string? AltTitles { get; }
        public string Description { get; }
    }
}
