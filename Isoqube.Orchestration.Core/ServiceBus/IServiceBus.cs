namespace Isoqube.Orchestration.Core.ServiceBus
{
    public interface IServiceBus
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
    }
}
