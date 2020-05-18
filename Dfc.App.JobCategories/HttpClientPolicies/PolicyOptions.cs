using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.HttpClientPolicies
{
    [ExcludeFromCodeCoverage]
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }
    }
}
