using Isoqube.Endpoint.HostedServices;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.Services;
using Isoqube.Orchestration.Core.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Isoqube.Endpoint
{
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSignalR();
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
                builder.WithOrigins("http://localhost:4999").AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials();
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/api/health/ready");
                endpoints.MapHealthChecks("/api/health/live");
                endpoints.MapHub<NotifyClientsHub>("/notifyclientshub");
            });
        }
    }
}
