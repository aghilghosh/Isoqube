using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;

namespace Isoqube.Orchestration.Core.ServiceBus.Topics
{
    [EntityName("windows-2019-build")]
    [TopicName("Windows2019Build", "Build Windows 2019")]
    public class Windows2019Build(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("windows-2022-build")]
    [TopicName("Windows2022Build", "Build Windows 2022")]
    public class Windows2022Build(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("windows-2025-build")]
    [TopicName("Windows2025Build", "Build Windows 2025")]
    public class Windows2025Build(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("windows-build-completed")]
    [TopicName("WindowsBuildCompleted", "Windows build completed")]
    public class WindowsBuildCompleted(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }
}
