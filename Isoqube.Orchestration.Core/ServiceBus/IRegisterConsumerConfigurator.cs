namespace Isoqube.Orchestration.Core.ServiceBus
{
    public interface IRegisterConsumerConfigurator<IConsumer>
    {
        void AddMasstransit(List<Type>? consumers = null);
    }
}
