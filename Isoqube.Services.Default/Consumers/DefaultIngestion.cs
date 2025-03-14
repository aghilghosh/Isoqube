using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.Default.Consumers
{
    [ConsumerName("DefaultIngestionConsumer")]
    public class DefaultIngestionConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<DefaultIngestionConsumer> logger) : IConsumer<DefaultSourceIngestion>
    {
        private readonly ILogger<DefaultIngestionConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<DefaultSourceIngestion> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            _logger.LogInformation($"DefaultIngestion consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));

            await Task.CompletedTask;
        }
    }
}
