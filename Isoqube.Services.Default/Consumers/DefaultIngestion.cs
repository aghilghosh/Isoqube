using Isoqube.Orchestration.Core.Configurations.Utilities;
using Isoqube.Orchestration.Core.ServiceBus;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.ServiceBus.Models;
using Isoqube.Orchestration.Core.ServiceBus.Topics;
using Isoqube.Orchestration.Core.SignalR;
using Isoqube.Orchestration.Core.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
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

    [ConsumerName("EventNotificationConsumer")]
    public class EventNotificationConsumer(IHubContext<NotifyClientsHub> hubContext, ILogger<EventNotificationConsumer> logger) : IConsumer<EventNotification>
    {
        public async Task Consume(ConsumeContext<EventNotification> context)
        {
            if (context is not null)
            {
                logger.LogInformation($"EventNotification consumer: CorrelationId: {context.Message.CorrelationId}, Ingestion {context.Message.IngestionId} received");                
                hubContext.Clients.All.SendAsync("listentonotifications", new NotifyClient { CurrentTopic = context.Message?.CurrentTopic, RunId = context.Message.IngestionId });
            }

            await Task.CompletedTask;
        }
    }
}
