using MassTransit;
using MassTransit.SqlTransport.Topology;
using Isoqube.Orchestration.Core.Configurations.Models;
using Isoqube.Orchestration.Core.Configurations.Utilities;
using Isoqube.Orchestration.Core.Data;
using Isoqube.Orchestration.Core.Data.Entities;
using Isoqube.Orchestration.Core.Extensions;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.Services;
using Isoqube.Orchestration.Core.Utilities;
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

            // MongoDb with application logging service settings
            AddMongoDbWithApplicationLoggingService(collection);

            // Enable isolated logging services
            EnableIsolatedLogging(collection);

            // Masstransit service bus settings https://medium.com/@gabrieletronchin/c-net-cloud-agnostic-service-bus-implementation-with-masstransit-b9dff03eb0f3
            Bootstrapper<TEnvSettings, TAppSettings, TJobConfig>.AddMasstransit(collection);
            AddTopics(collection);
        }

        private static void EnableIsolatedLogging(IServiceCollection collection)
        {
            if (AppSettingsBase.Logger.EnableIsolatedLogging)
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

        private static void AddMongoDbWithApplicationLoggingService(IServiceCollection collection)
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

                if (!AppSettingsBase.Logger.EnableIsolatedLogging)
                {
                    collection.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.AddMongoDbLogger(new MongoConfiguration()
                        {
                            logCollection = database.GetCollection<Log>(AppSettingsBase.Mongo.LogCollectionName),
                            MinLevel = AppSettingsBase.Mongo.LogLevel.Value
                        }, Environment.GetEnvironmentVariable("APPLICATION_NAME").ToUpper() ?? Assembly.GetCallingAssembly().GetName().Name, Environment.GetEnvironmentVariable("ENVIRONMENT"));
                    });
                }
            }
        }

        private static void AddMasstransit(IServiceCollection collection)
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
                        logger?.LogInformation($"Consumer attached: {consumer.Namespace}.{consumer.Name}");
                    }
                }

                configure.SetKebabCaseEndpointNameFormatter();

                switch (ServiceBusOptions.ServiceBusType)
                {
                    case ServiceBusType.InMemory:
                        configure.UsingInMemory(
                            (context, cfg) =>
                            {
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
                                cfg.ConfigureEndpoints(context);
                            }
                        );

                        break;

                    default:
                        break;
                }
            });
        }

        private static async void AddTopics(IServiceCollection collection)
        {
            var mongoDatabase = collection.BuildServiceProvider().GetService<IMongoDatabase>();
            var topicCollection = mongoDatabase.GetCollection<RegisteredTopic>("RegisteredTopics");

            // Ensure the unique index on Email is created
            var indexKeys = Builders<RegisteredTopic>.IndexKeys.Ascending(topic => topic.Name);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<RegisteredTopic>(indexKeys, indexOptions);
            topicCollection.Indexes.CreateOne(indexModel);

            var registeredTopics = ReflectionBrocker.GetTopics(true);

            if (registeredTopics is not null)
            {
                Type attributeType = typeof(TopicName);
                var topics = registeredTopics.Select(type => new
                    {
                        ClassName = type.Name,
                        AttributeDescription = ((TopicName)Attribute.GetCustomAttribute(type, attributeType))?.Description ?? type.Name
                    });

                await SaveToCollections(topicCollection, topics.Select(topic => new RegisteredTopic
                {
                    CreatedOn = PlatformDateTime.Datetime,
                    Description = topic.AttributeDescription,
                    Type = topic.ClassName.Split('`')[0],
                    Name = topic.ClassName.Split('`')[0],
                    Version = Environment.GetEnvironmentVariable("VERSION") ?? "1.0.0"
                }));
            }
        }

        private static async Task SaveToCollections(IMongoCollection<RegisteredTopic> topicCollection, IEnumerable< RegisteredTopic> registerTopics)
        {
            try
            {
                await topicCollection.InsertManyAsync(registerTopics);
            }
            catch (Exception ex)
            {
                if (ex is not null && ex.Message.Contains("E11000 duplicate key error collection"))
                {
                    return;
                }
            }
        }
    }
}