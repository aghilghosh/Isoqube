using MassTransit;
using Isoqube.Orchestration.Core.Configurations.Utilities;
using Isoqube.Orchestration.Core.Data.Entities;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Reflection;

namespace Isoqube.Orchestration.Core.Utilities
{
    public static class TopicResolver
    {
        public static object? Resolve(string topic, TopicBase context)
        {
            return CreateInstanceBinder(topic, context);
        }

        private static object? CreateInstanceBinder(string _topic, TopicBase context)
        {
            var topics = ReflectionBrocker.GetTopics();

            if (topics is not null && topics.Any())
            {
                var matchingType = topics.Where(type =>
                {
                    var attribute = type.GetCustomAttribute<TopicName>();
                    return attribute != null && attribute.Name == _topic;
                }).FirstOrDefault();

                if (matchingType is not null)
                {
                    var dynamicTopic = Activator.CreateInstance(matchingType, context.Timestamp, context.CorrelationId, context.IngestionId);
                    return dynamicTopic;
                }
            }

            return null;
        }

        public static async Task HandleTopic(IMongoDatabase mongoDb, IServiceBus serviceBus, ConsumeContext<TopicBase> context)
        {
            try
            {
                object configuredTopic = null;
                var configuredRunCollection = mongoDb.GetCollection<RunEntity>("RunEntity");
                var configuredRunAsync = await configuredRunCollection.FindAsync(Builders<RunEntity>.Filter.Eq(e => e.Id, context.Message.CorrelationId));
                var configuredRun = (await configuredRunAsync.ToListAsync()).FirstOrDefault();

                if (configuredRun is not null)
                {
                    var currentTopic = configuredRun.Topics.FirstOrDefault(topic => topic.Name == context.Message.GetType().Name);

                    if (currentTopic is not null)
                    {
                        currentTopic.CompletedOn = PlatformDateTime.Datetime;

                        var filter = Builders<RunEntity>.Filter.Eq(e => e.Id, configuredRun.Id);

                        var updateDefinition = Builders<RunEntity>.Update
                            .Set(e => e.ModifiedOn, PlatformDateTime.Datetime)
                            .Set(e => e.Topics.ElementAt(configuredRun.Topics.ToList().IndexOf(currentTopic)), currentTopic);

                        await configuredRunCollection.UpdateOneAsync(filter, updateDefinition);

                        var nextTopic = configuredRun.Topics.FirstOrDefault(topic => topic.CompletedOn is null);

                        configuredTopic = Resolve(nextTopic.Name, context.Message);
                        if (configuredTopic is null)
                        {
                            return;
                        }
                    }
                }

                await serviceBus.PublishAsync(configuredTopic);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
