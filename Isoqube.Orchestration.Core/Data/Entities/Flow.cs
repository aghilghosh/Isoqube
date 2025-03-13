using Isoqube.SharedServices.Framework.Models;

namespace Isoqube.Orchestration.Core.Data.Entities
{
    public class Flow : Entity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
    }

    public class Job
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
