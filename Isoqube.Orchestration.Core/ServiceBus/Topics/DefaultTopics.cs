using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using Isoqube.Orchestration.Core.Data.Entities;

namespace Isoqube.Orchestration.Core.ServiceBus.Topics
{
    [EntityName("default-source-ingestion")]
    [TopicName("DefaultSourceIngestion", "Default ingestion")]
    public class DefaultSourceIngestion(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
        public string? PredefinedFlow { get; set; }
        public IEnumerable<string>? Flow { get; set; }
    }

    [EntityName("default-source-download")]
    [TopicName("DefaultSourceDownload", "Source download")]
    public class DefaultSourceDownload(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
        public string? PredefinedFlow { get; set; }
        public IEnumerable<string>? Flow { get; set; }
    }

    [EntityName("default-source-download-complete")]
    [TopicName("DefaultSourceDownloadComplete", "Source download completed")]
    public class DefaultSourceDownloadComplete(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
        public string? PredefinedFlow { get; set; }
        public IEnumerable<string>? Flow { get; set; }
    }

    [EntityName("event-notification")]
    [TopicName("EventNotification", "Event notifications")]
    public class EventNotification(RunEntity run, DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
        public RunEntity Run { get; set; } = run;
    }
}
