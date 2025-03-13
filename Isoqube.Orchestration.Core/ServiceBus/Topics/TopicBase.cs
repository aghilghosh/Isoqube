namespace Isoqube.Orchestration.Core.ServiceBus.Topics
{
    public class TopicBase(DateTime timestamp, string correlationId, string ingestionId)
    {
        public DateTime Timestamp { get; } = timestamp;
        public string CorrelationId { get; set; } = correlationId;
        public string IngestionId { get; set; } = ingestionId;
    }
}
