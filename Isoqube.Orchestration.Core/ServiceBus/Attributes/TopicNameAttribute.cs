namespace Isoqube.Orchestration.Core.ServiceBus.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TopicName(string name, string? description) : Attribute
    {
        public string Name { get; } = name;

        public string? Description { get; } = description;
    }
}
