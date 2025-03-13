using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Services.Default.Consumers
{
    [ConsumerName("DetermineBuildTypeConsumer")]
    public class DetermineBuildTypeConsumer(IServiceBus serviceBus, IMongoDatabase mongoDb, ILogger<DetermineBuildTypeConsumer> logger) : IConsumer<DefaultSourceDownloadComplete>
    {
        private readonly ILogger<DetermineBuildTypeConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<DefaultSourceDownloadComplete> context)
        {
            _logger.LogInformation($"DetermineBuildType consumer: CorrelationId: {context.Message.CorrelationId}, payload {context.Message.IngestionId} received");
            await Task.Delay(TimeSpan.FromSeconds(5));


            context.AddConsumeTask(TopicResolver.HandleTopic(mongoDb, serviceBus, context));

            await Task.CompletedTask;
        }
    }
}
