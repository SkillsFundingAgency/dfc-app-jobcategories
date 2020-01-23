using System;
using System.Collections.Generic;

namespace Dfc.App.JobCategories.Data.Models
{
    public class CategoryDataModel
    {
        public DateTime LastReviewed { get; set; }

        public string CanonicalName { get; set; }

        public List<JobProfileDataModel> JobProfiles { get; set; }
    }
}