using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.Default.Consumers
{
    [ConsumerName("SourceDownload")]
    public class SourceDownload(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<SourceDownload> logger) : IConsumer<DefaultSourceDownload>
    {
        private readonly ILogger<SourceDownload> _logger = logger;

        public async Task Consume(ConsumeContext<DefaultSourceDownload> context)
        {
            await TopicResolver.InvokeTopic(mongoDb, serviceBus, context);
            _logger.LogInformation($"SourceDownload consumer: CorrelationId: {context.Message.CorrelationId}, payload {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));

            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));
            await Task.CompletedTask;
        }
    }
}
