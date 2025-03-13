using Isoqube.SharedServices.Framework.Models;

namespace Isoqube.Orchestration.Core.Data.Entities
{
    public class RegisteredTopic : PlatformEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
    }

    public class TopicRun : PlatformEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime? FailedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime? CancelledOn { get; set; }
        public DateTime? RetriedOn { get; set; }
        public DateTime? SucceededOn { get; set; }
        public DateTime? TriggeredOn { get; set; }
    }
}
