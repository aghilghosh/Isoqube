using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.WindowsPool.Consumers
{
    [ConsumerName("Windows2019BuildConsumer")]
    public class Windows2019BuildConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<Windows2019BuildConsumer> logger) : IConsumer<Windows2019Build>
    {
        public async Task Consume(ConsumeContext<Windows2019Build> context)
        {
            logger.LogInformation($"Windows2019Build consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("Windows2022BuildConsumer")]
    public class Windows2022BuildConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<Windows2022BuildConsumer> logger) : IConsumer<Windows2022Build>
    {
        public async Task Consume(ConsumeContext<Windows2022Build> context)
        {
            logger.LogInformation($"Windows2022Build consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }

    [ConsumerName("Windows2025BuildConsumer")]
    public class Windows2025BuildConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<Windows2025BuildConsumer> logger) : IConsumer<Windows2025Build>
    {
        public async Task Consume(ConsumeContext<Windows2025Build> context)
        {
            logger.LogInformation($"Windows2025Build Consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }
}
