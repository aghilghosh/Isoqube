using Isoqube.Orchestration.Core.Data.Entities;

namespace Isoqube.Orchestration.Core.ServiceBus.Models
{
    public class Ingestion
    {
        public string ExternalId { get; set; }
        public string? FlowId { get; set; }
    }

    public class Configuration
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<RegisteredTopic> Topics { get; set; }
    }

    public class ConfigurationRun
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public Configuration RunConfiguration { get; set; }
    }
}
