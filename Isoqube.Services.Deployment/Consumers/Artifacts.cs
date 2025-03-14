using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.Deployment.Consumers
{
    [ConsumerName("WindowsBuildCompletedConsumer")]
    public class WindowsBuildCompletedConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<WindowsBuildCompletedConsumer> logger) : IConsumer<WindowsBuildCompleted>
    {
        public async Task Consume(ConsumeContext<WindowsBuildCompleted> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            logger.LogInformation($"WindowsBuildCompleted consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("LinuxBuildCompletedConsumer")]
    public class LinuxBuildCompletedConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<LinuxBuildCompletedConsumer> logger) : IConsumer<LinuxBuildCompleted>
    {
        public async Task Consume(ConsumeContext<LinuxBuildCompleted> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            logger.LogInformation($"LinuxBuildCompleted consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("BroadcastReleaseConsumer")]
    public class BroadcastReleaseConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<BroadcastReleaseConsumer> logger) : IConsumer<BroadcastRelease>
    {
        public async Task Consume(ConsumeContext<BroadcastRelease> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            logger.LogInformation($"BroadcastRelease consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }
}
