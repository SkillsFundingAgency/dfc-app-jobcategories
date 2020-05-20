using Microsoft.AspNetCore.Html;
using System.Collections.Generic;

namespace DFC.App.JobCategories.ViewModels
{
    public class BodyViewModel
    {
        public HtmlString? Content { get; set; }

        public string? Category { get; set; }

        public IEnumerable<JobProfileListItemViewModel> Profiles { get; set; } = new List<JobProfileListItemViewModel>();
    }
}
