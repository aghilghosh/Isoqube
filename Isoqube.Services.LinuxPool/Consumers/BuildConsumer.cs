using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.LinuxPool.Consumers
{
    [ConsumerName("LinuxUbundu2004BuildConsumer")]
    public class LinuxUbundu2004BuildConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<LinuxUbundu2004BuildConsumer> logger) : IConsumer<LinuxUbundu2004Build>
    {
        public async Task Consume(ConsumeContext<LinuxUbundu2004Build> context)
        {
            logger.LogInformation($"LinuxUbundu2004Build consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("LinuxUbundu2204BuildConsumer")]
    public class LinuxUbundu2204BuildConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<LinuxUbundu2204BuildConsumer> logger) : IConsumer<LinuxUbundu2204Build>
    {
        public async Task Consume(ConsumeContext<LinuxUbundu2204Build> context)
        {
            logger.LogInformation($"LinuxUbundu2204BuildConsumer consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("LinuxUbundu2404BuildConsumer")]
    public class LinuxUbundu2404BuildConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<LinuxUbundu2404BuildConsumer> logger) : IConsumer<LinuxUbundu2404Build>
    {
        public async Task Consume(ConsumeContext<LinuxUbundu2404Build> context)
        {
            logger.LogInformation($"LinuxUbundu2404Build Consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }
}
