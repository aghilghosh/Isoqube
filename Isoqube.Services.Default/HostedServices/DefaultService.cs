using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.Services;

namespace Isoqube.Endpoint.HostedServices
{
    public class DefaultService(IServiceBus serviceBus, HostedServiceHealthCheck hostedServiceHealthCheck) : BackgroundServiceBrocker(hostedServiceHealthCheck)
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(
                Task.Run(() => { Console.WriteLine($"{Environment.GetEnvironmentVariable("APPLICATION_NAME")} service started"); }, stoppingToken),
                base.ExecuteAsync(stoppingToken),
                Task.Run(() => { Console.WriteLine($"{Environment.GetEnvironmentVariable("APPLICATION_NAME")} service stopped"); }, stoppingToken));
        }
    }
}
