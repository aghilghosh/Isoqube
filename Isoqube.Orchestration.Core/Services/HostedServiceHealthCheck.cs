using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Isoqube.Orchestration.Core.Services
{
    public class HostedServiceHealthCheck : IHealthCheck
    {
        private readonly List<string> _availableServices = new();
        private readonly List<string> _startedServices = new();

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_availableServices.Count() == _startedServices.Count())
            {
                return Task.FromResult(HealthCheckResult.Healthy("All services started"));
            }

            var failedServices = _availableServices.Except(_startedServices);

            return Task.FromResult(HealthCheckResult.Unhealthy($"Services {string.Join(',', failedServices)} either not started or terminated"));
        }

        public void AddAvailableService(string serviceName)
        {
            _availableServices.Add(serviceName);
        }

        public void AddStartedService(string serviceName)
        {
            _startedServices.Add(serviceName);
        }

        public void AddStoppedService(string serviceName)
        {
            _startedServices.Remove(serviceName);
        }
    }
}
