using Isoqube.SharedServices.Framework.Models;

namespace Isoqube.Orchestration.Core.Data.Entities
{
    public class ConfigurationEntity : PlatformEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<RegisteredTopic> Topics { get; set; }
    }

    public class RunEntity : PlatformEntity
    {
        public string? Description { get; set; }
        public IEnumerable<TopicRun> Topics { get; set; }
    }
}
