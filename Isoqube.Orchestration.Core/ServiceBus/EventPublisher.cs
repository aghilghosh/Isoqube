using MassTransit;

namespace Isoqube.Orchestration.Core.ServiceBus
{
    public class EventPublisher(IPublishEndpoint bus) : IServiceBus
    {
        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            await bus.Publish(message, cancellationToken);
        }
    }
}
