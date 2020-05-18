using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.HttpClientPolicies
{
    [ExcludeFromCodeCoverage]
    public class RetryPolicyOptions
    {
        public int Count { get; set; } = 3;

        public int BackoffPower { get; set; } = 2;
    }
}
