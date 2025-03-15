using Isoqube.Orchestration.Core.Data.Entities;

namespace Isoqube.Orchestration.Core.ServiceBus.Models
{
    public class NotifyClient
    {
        public string RunId { get; set; }
        public TopicRun? CurrentTopic { get; set; }
    }
}
