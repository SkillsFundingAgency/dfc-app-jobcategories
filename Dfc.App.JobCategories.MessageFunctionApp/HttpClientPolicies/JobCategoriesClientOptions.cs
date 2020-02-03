using System;

namespace Dfc.App.JobCategories.MessageFunctionApp.HttpClientPolicies
{
    public class JobCategoriesClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);
    }
}