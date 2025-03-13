using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;

namespace Isoqube.Orchestration.Core.ServiceBus.Topics
{
    [EntityName("linux-ubundu-2004-build")]
    [TopicName("LinuxUbundu2004Build", "Build Linux Ubundu 2004")]
    public class LinuxUbundu2004Build(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("linux-ubundu-2204-build")]
    [TopicName("LinuxUbundu2204Build", "Build Linux Ubundu 2204")]
    public class LinuxUbundu2204Build(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("linux-ubundu-2404-build")]
    [TopicName("LinuxUbundu2404Build", "Build Linux Ubundu 2404")]
    public class LinuxUbundu2404Build(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("linux-build-completed")]
    [TopicName("LinuxBuildCompleted", "Linux build completed")]
    public class LinuxBuildCompleted(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }
}
