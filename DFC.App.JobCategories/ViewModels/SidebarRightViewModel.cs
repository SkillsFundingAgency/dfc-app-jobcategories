using System.Collections.Generic;

namespace DFC.App.JobCategories.ViewModels
{
    public class SidebarRightViewModel
    {
        public Dictionary<string, string> Categories { get; set; } = new Dictionary<string, string>
        {
            { "Animal care", "/job-categories/animal-care" },
            { "Beauty and wellbeing", "/job-categories/beauty-and-wellbeing" },
            { "Business and finance", "/job-categories/business-and-finance" },
            { "Computing, technology and digital", "/job-categories/computing-technology-and-digital" },
            { "Construction and trades", "/job-categories/construction-and-trades" },
            { "Creative and media", "/job-categories/creative-and-media" },
        };
    }
}
