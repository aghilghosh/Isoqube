using Microsoft.Extensions.Hosting;

namespace Isoqube.Orchestration.Core.Services
{
    public abstract class BackgroundServiceBrocker : BackgroundService, IBackgroundService
    {
        private readonly HostedServiceHealthCheck _hostedServiceHealthCheck;

        protected BackgroundServiceBrocker(HostedServiceHealthCheck hostedServiceHealthCheck)
        {
            _hostedServiceHealthCheck = hostedServiceHealthCheck;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var serviceName = GetType().Name;
            CustomConsoleWrite($"{serviceName} service started");
            _hostedServiceHealthCheck.AddStartedService(serviceName);
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            var serviceName = GetType().Name;
            CustomConsoleWrite($"{serviceName} service stopped");
            _hostedServiceHealthCheck.AddStoppedService(serviceName);
            return Task.CompletedTask;
        }

        private void CustomConsoleWrite(string message, ConsoleColor? consoleColor = null)
        {
            Console.BackgroundColor = consoleColor ?? ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public interface IBackgroundService : IHostedService
    {
    }
}
