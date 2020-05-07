namespace DFC.App.JobCategories.ViewModels
{
    public class JobProfileListItemViewModel
    {
        public JobProfileListItemViewModel(string title, string url, string? altTitles, string description)
        {
            Title = title;
            URL = url;
            AltTitles = altTitles;
            Description = description;
        }

        public string Title { get; }
        public string URL { get; }
        public string? AltTitles { get; }
        public string Description { get; }
    }
}
