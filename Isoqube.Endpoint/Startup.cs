using Isoqube.Endpoint.HostedServices;
using Isoqube.Orchestration.Core.Configurations.Utilities;
using Isoqube.Orchestration.Core.Data.Entities;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Models;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

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
                endpoints.MapHealthChecks("/api/health/live");

                endpoints.MapGet("/api/registeredtopics", async (IMongoDatabase mongoDb) =>
                {
                    try
                    {
                        var registeredTopics = await mongoDb.GetCollection<RegisteredTopic>("RegisteredTopics").FindAsync(Builders<RegisteredTopic>.Filter.Empty);
                        var topicsList = await registeredTopics.ToListAsync();
                        return Results.Ok(topicsList);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(ex.Message);
                    }
                });

                endpoints.MapPost("/api/configuration/run", async (IServiceBus servicebus, IMongoDatabase mongoDb, [FromBody] ConfigurationRun runConfiguration) =>
                {
                    try
                    {
                        RunEntity runEntity = null;

                        var configurationCollection = mongoDb.GetCollection<RunEntity>("RunEntity");

                        if (runConfiguration.Id is null)
                        {
                            runEntity = new RunEntity()
                            {
                                Description = runConfiguration.Description,
                                CreatedOn = PlatformDateTime.Datetime,
                                Topics = runConfiguration.RunConfiguration.Topics.Select(topic => new TopicRun()
                                {
                                    Name = topic.Name,
                                    Type = topic.Type,
                                    Description = topic.Description,
                                    Version = topic.Version
                                })
                            };

                            await configurationCollection.InsertOneAsync(runEntity);
                        }
                        else {

                            var existingRunConfiguration = await configurationCollection.FindAsync(Builders<RunEntity>.Filter.Eq(e => e.Id, runConfiguration.Id));
                            runEntity = (await existingRunConfiguration.ToListAsync()).First();

                            var filter = Builders<RunEntity>.Filter.Eq(e => e.Id, runEntity.Id);
                            var update = Builders<RunEntity>.Update
                            .Set(e => e.ModifiedOn, PlatformDateTime.Datetime);

                            await configurationCollection.UpdateOneAsync(filter, update);
                        }
                        
                        await servicebus.PublishAsync(new DefaultSourceIngestion(PlatformDateTime.Datetime, runEntity.Id, runEntity.Id));

                        return Results.Ok();
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(ex.Message);
                    }
                });

                endpoints.MapGet("/api/configuration/run/all", async (IMongoDatabase mongoDb) =>
                {
                    try
                    {
                        var configurationCollection = await mongoDb.GetCollection<RunEntity>("RunEntity").FindAsync(Builders<RunEntity>.Filter.Empty);
                        var configurations = await configurationCollection.ToListAsync();

                        return Results.Ok(configurations);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(ex.Message);
                    }
                });

                endpoints.MapPost("/api/configuration", async (IMongoDatabase mongoDb, [FromBody] Configuration configuration) =>
                {
                    try
                    {
                        var configurationCollection = mongoDb.GetCollection<ConfigurationEntity>("ConfigurationEntity");

                        if (!string.IsNullOrEmpty(configuration.Id))
                        {
                            var filter = Builders<ConfigurationEntity>.Filter.Eq(e => e.Id, configuration.Id);
                            var update = Builders<ConfigurationEntity>.Update
                                .Set(e => e.Name, configuration.Name)
                                .Set(e => e.Description, configuration.Description)
                                .Set(e => e.Topics, configuration.Topics);

                            await configurationCollection.UpdateOneAsync(filter, update);
                        }
                        else
                        {
                            await configurationCollection.InsertOneAsync(new ConfigurationEntity()
                            {
                                Name = configuration.Name,
                                Description = configuration.Description,
                                Topics = configuration.Topics
                            });
                        }

                        return Results.Ok();
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(ex.Message);
                    }
                });

                endpoints.MapGet("/api/configuration/all", async (IMongoDatabase mongoDb) =>
                {
                    try
                    {
                        var configurationCollection = await mongoDb.GetCollection<ConfigurationEntity>("ConfigurationEntity").FindAsync(Builders<ConfigurationEntity>.Filter.Empty);
                        var configurations = await configurationCollection.ToListAsync();
                        return Results.Ok(configurations);
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(ex.Message);
                    }
                });

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
