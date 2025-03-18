using Isoqube.Orchestration.Core.Configurations.Models;
using Isoqube.Orchestration.Core.Data;
using Isoqube.Orchestration.Core.Extensions;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.Services;
using Isoqube.Orchestration.Core.Utilities;
using MassTransit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Reflection;

namespace Isoqube.Orchestration.Core
{
    public class Bootstrapper<TEnvSettings, TAppSettings, TJobConfig>
        where TEnvSettings : EnvSettingsBase
        where TAppSettings : AppSettingsBase
        where TJobConfig : JobConfigurationBase
    {
        private IConfigurationRoot? _configurationRoot;

        public async Task BootstrapService<TStartup>(string[] args) where TStartup : class
        {
            var builder = new ConfigurationBuilder();

            builder.InitializeConfiguration();
            _configurationRoot = builder.Build();

            ArgumentNullException.ThrowIfNull(Environment.GetEnvironmentVariable("HOST"));

            await WebHost.CreateDefaultBuilder(args)
                 .ConfigureServices(AddServices)
                 .UseStartup<TStartup>()
                 .UseUrls(Environment.GetEnvironmentVariable("HOST") ?? "")
                 .UseDefaultServiceProvider(options => { options.ValidateOnBuild = true; })
                 .Build()
                 .RunAsync();
        }

        private void AddServices(IServiceCollection collection)
        {
            collection.AddSingleton(this);

            // Health check
            var healthCheckBuilder = collection.AddHealthChecks();
            collection.AddSingleton<HostedServiceHealthCheck>();
            healthCheckBuilder.AddCheck<HostedServiceHealthCheck>("BackgroundServices", Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy);

            // App Settings
            var appSettings = _configurationRoot?.GetSection("AppSettings").Get<TAppSettings>();
            if (appSettings is not null) collection.AddSingleton(appSettings);

            // App Settings
            var envSettings = _configurationRoot?.GetSection("EnvSettings").Get<TEnvSettings>();
            if (envSettings is not null) collection.AddSingleton(envSettings);

            // App Settings
            var jobConfiguration = _configurationRoot?.GetSection("JobConfiguration").Get<TJobConfig>();
            if (jobConfiguration is not null) collection.AddSingleton(jobConfiguration);

            collection.AddTransient<IServiceBus, EventPublisher>();

            // Masstransit service bus settings https://medium.com/@gabrieletronchin/c-net-cloud-agnostic-service-bus-implementation-with-masstransit-b9dff03eb0f3
            AddConsumers(collection);

            // MongoDb with application logging service settings
            AddMongoDb(collection);

            // Enable MongoDb logging services
            EnableMongoDbLogging(collection);
        }

        private static void EnableMongoDbLogging(IServiceCollection collection)
        {
            if (AppSettingsBase.Logger != null && AppSettingsBase.Logger.EnableIsolatedLogging)
            {
                ArgumentNullException.ThrowIfNull(nameof(AppSettingsBase.Logger.Provider));

                var mongoDbSettings = MongoClientSettings.FromConnectionString(AppSettingsBase.Logger.Provider.MongoDb.ConnectionString);
                if (!string.IsNullOrEmpty(AppSettingsBase.Logger.Provider.MongoDb.UserName) && !string.IsNullOrEmpty(AppSettingsBase.Logger.Provider.MongoDb.Password) && !string.IsNullOrEmpty(AppSettingsBase.Logger.Provider.MongoDb.DatabaseName))
                {
                    mongoDbSettings.Credential = MongoCredential.CreateCredential(AppSettingsBase.Logger.Provider.MongoDb.DatabaseName, AppSettingsBase.Logger.Provider.MongoDb.UserName, AppSettingsBase.Logger.Provider.MongoDb.Password);
                }

                var mongoClient = new MongoClient(mongoDbSettings);
                var database = mongoClient.GetDatabase(AppSettingsBase.Logger.Provider.MongoDb.DatabaseName);

                collection.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddMongoDbLogger(new MongoConfiguration()
                    {
                        logCollection = database.GetCollection<Log>(AppSettingsBase.Logger.Provider.MongoDb.LogCollectionName),
                        MinLevel = AppSettingsBase.Logger.Provider.MongoDb.LogLevel.Value
                    }, Environment.GetEnvironmentVariable("APPLICATION_NAME").ToUpper() ?? Assembly.GetCallingAssembly().GetName().Name, Environment.GetEnvironmentVariable("ENVIRONMENT"));
                });
            }
        }

        private static void AddMongoDb(IServiceCollection collection)
        {
            if (AppSettingsBase.Mongo is not null && !string.IsNullOrEmpty(AppSettingsBase.Mongo.ConnectionString) && !string.IsNullOrEmpty(AppSettingsBase.Mongo.DatabaseName))
            {
                var mongoDbSettings = MongoClientSettings.FromConnectionString(AppSettingsBase.Mongo.ConnectionString);
                if (!string.IsNullOrEmpty(AppSettingsBase.Mongo.UserName) && !string.IsNullOrEmpty(AppSettingsBase.Mongo.Password) && !string.IsNullOrEmpty(AppSettingsBase.Mongo.DatabaseName))
                {
                    mongoDbSettings.Credential = MongoCredential.CreateCredential(AppSettingsBase.Mongo.DatabaseName, AppSettingsBase.Mongo.UserName, AppSettingsBase.Mongo.Password);
                }

                var mongoClient = new MongoClient(mongoDbSettings);
                var database = mongoClient.GetDatabase(AppSettingsBase.Mongo.DatabaseName);
                collection.AddSingleton(database);
            }
        }

        private static void AddConsumers(IServiceCollection collection)
        {
            var logger = collection.BuildServiceProvider().GetService<ILogger<Bootstrapper<TEnvSettings, TAppSettings, TJobConfig>>>();

            collection.AddMassTransit(configure =>
            {
                var registeredConsumers = ReflectionBrocker.GetConsumers();

                if (registeredConsumers != null && registeredConsumers.Any())
                {
                    foreach (var consumer in registeredConsumers)
                    {
                        configure.AddConsumer(consumer);
                        logger?.LogInformation("Consumer attached: {ConsumerNamespace}.{ConsumerName}", consumer.Namespace, consumer.Name);
                    }
                }

                configure.SetKebabCaseEndpointNameFormatter();

                switch (ServiceBusOptions.ServiceBusType)
                {
                    case ServiceBusType.InMemory:
                        configure.UsingInMemory(
                            (context, cfg) =>
                            {
                                cfg.UseMessageRetry(retryConfig =>
                                {
                                    retryConfig.Interval(3, TimeSpan.FromSeconds(5));
                                });

                                cfg.ConfigureEndpoints(context);
                            }
                        );
                        break;

                    case ServiceBusType.RabbitMQ:

                        ArgumentNullException.ThrowIfNull(AppSettingsBase.RabbitMq);

                        configure.AddDelayedMessageScheduler();
                        configure.UsingRabbitMq((context, cfg) =>
                            {
                                cfg.Host(AppSettingsBase.RabbitMq?.ConnectionString, AppSettingsBase.RabbitMq?.VirtualHost, host =>
                                {
                                    host.Username(AppSettingsBase.RabbitMq?.UserName ?? string.Empty);
                                    host.Password(AppSettingsBase.RabbitMq?.Password ?? string.Empty);
                                });
                                cfg.UseCircuitBreaker(cb =>
                                {
                                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                                    cb.TripThreshold = 15;
                                    cb.ActiveThreshold = 10;
                                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                                });
                                cfg.UseMessageRetry(retryConfig =>
                                {
                                    retryConfig.Interval(3, TimeSpan.FromSeconds(5));
                                });

                                cfg.ConfigureEndpoints(context);
                            }
                        );

                        break;

                    default:
                        break;
                }
            });
        }
    }
}