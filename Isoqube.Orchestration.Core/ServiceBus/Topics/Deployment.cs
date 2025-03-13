using MassTransit;
using Isoqube.Orchestration.Core.ServiceBus.Attributes;

namespace Isoqube.Orchestration.Core.ServiceBus.Topics
{
    [EntityName("deploy-to-vm")]
    [TopicName("DeployToVm", "Deploy to VM")]
    public class DeployToVm(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("deploy-to-container")]
    [TopicName("DeployToContainer", "Deploy to container")]
    public class DeployToContainer(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("deployment-completed")]
    [TopicName("DeploymentCompleted", "Deployment completed")]
    public class Deploymentcompleted(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }

    [EntityName("broadcast-release")]
    [TopicName("BroadcastRelease", "Broadcast release")]
    public class BroadcastRelease(DateTime Timestamp, string CorrelationId, string IngestionId) : TopicBase(Timestamp, CorrelationId, IngestionId)
    {
    }
}
