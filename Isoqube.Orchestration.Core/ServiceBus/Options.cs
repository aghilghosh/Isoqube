namespace Isoqube.Orchestration.Core.ServiceBus
{
    public class ServiceBusOptions
    {
        public static ServiceBusType ServiceBusType { get; set; }
    }

    public enum ServiceBusType
    {
        InMemory,
        AzureBus,
        RabbitMQ
    }
}
