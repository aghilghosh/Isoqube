using Isoqube.Services.WindowsPool.HostedServices;
using Isoqube.Orchestration.Core.Configurations.Utilities;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Models;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Isoqube.Endpoint
{
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddTransient<IServiceBus, EventPublisher>();
            services.AddHostedService<DefaultService>();
        }

        public virtual void Configure(IApplicationBuilder app, HostedServiceHealthCheck hostedServiceHealthCheck)
        {
            var backgroundServices = app.ApplicationServices.GetServices<IHostedService>();
            backgroundServices.Where(service => service.GetType().IsAssignableTo(typeof(IBackgroundService)))
                .Select(service => service.GetType().Name).ToList()
                .ForEach(hostedServiceHealthCheck.AddAvailableService);

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4999").AllowAnyMethod().AllowAnyHeader();
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health/ready");
                endpoints.MapHealthChecks("/api/health/retry");

                endpoints.MapPost("/api/init", async (IServiceBus servicebus, [FromBody] Ingestion ingestionRequest) =>
                {
                    var correlationId = Guid.NewGuid().ToString();
                    Console.WriteLine($"CorrelationId: {correlationId}");
                    Console.WriteLine($"IngestionId: {ingestionRequest.ExternalId}");
                    await servicebus.PublishAsync(new DefaultSourceIngestion(PlatformDateTime.Datetime, correlationId, ingestionRequest.ExternalId));
                });
            });
        }
    }
}
