using System;

namespace Dfc.App.JobCategories.Data.Models
{
    public class JobProfileDataModel
    {
        public Guid JobProfileId { get; set; }

        public string CanonicalName { get; set; }

        public string OverviewText { get; set; }
    }
}