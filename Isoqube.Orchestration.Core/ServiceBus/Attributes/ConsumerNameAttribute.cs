namespace Isoqube.Orchestration.Core.ServiceBus.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ConsumerName : Attribute
    {
        public string Name { get; }

        public ConsumerName(string name)
        {
            Name = name;
        }
    }
}
