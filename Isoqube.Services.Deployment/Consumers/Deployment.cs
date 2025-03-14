using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.WindowsPool.Consumers
{
    [ConsumerName("DeployToVmConsumer")]
    public class DeployToVmConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<DeployToVmConsumer> logger) : IConsumer<DeployToVm>
    {
        public async Task Consume(ConsumeContext<DeployToVm> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            logger.LogInformation($"DeployToVm consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("DeployToContainerConsumer")]
    public class DeployToContainerConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<DeployToContainerConsumer> logger) : IConsumer<DeployToContainer>
    {
        public async Task Consume(ConsumeContext<DeployToContainer> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            logger.LogInformation($"DeployToContainer consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("DeploymentcompletedConsumer")]
    public class DeploymentcompletedConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<DeploymentcompletedConsumer> logger) : IConsumer<DeploymentCompleted>
    {
        public async Task Consume(ConsumeContext<DeploymentCompleted> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            logger.LogInformation($"Deploymentcompleted consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
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
