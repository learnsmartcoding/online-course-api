using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LSC.OnlineCourse.API.Common
{
    public class PrivateMemoryHealthCheck : IHealthCheck
    {
        private readonly long _maxMemoryThreshold;

        public PrivateMemoryHealthCheck(long maxMemoryThreshold)
        {
            _maxMemoryThreshold = maxMemoryThreshold;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            long memoryUsage = GC.GetTotalMemory(false);

            if (memoryUsage < _maxMemoryThreshold)
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Memory usage is under control: {memoryUsage / 1024 / 1024} MB."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy($"Memory usage is too high: {memoryUsage / 1024 / 1024} MB."));
        }
    }

}
