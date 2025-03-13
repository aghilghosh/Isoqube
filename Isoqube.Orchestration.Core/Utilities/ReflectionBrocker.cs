using Isoqube.Orchestration.Core.ServiceBus.Attributes;
using System.Collections.Concurrent;
using System.Reflection;

namespace Isoqube.Orchestration.Core.Utilities
{
    public static class ReflectionBrocker
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<Type>> _topicCache = new();

        public static IEnumerable<Type> GetConsumers()
        {
            var assemblies = Assembly.GetEntryAssembly().GetTypes();
            Type attributeType = typeof(ConsumerName);

            if (assemblies is not null)
            {
                return assemblies.Where(type => type.IsClass && type.GetCustomAttributes(attributeType, false).Any());
            }

            return [];
        }

        public static IEnumerable<Type> GetTopics(bool enableCaching = true)
        {
            if (enableCaching && _topicCache.TryGetValue("topics", out var cachedTopics))
            {
                return cachedTopics;
            }

            Type attributeType = typeof(TopicName);
            var executingAssemblies = Assembly.GetExecutingAssembly();

            if (executingAssemblies is not null)
            {
                var classesWithAttribute = executingAssemblies.GetTypes().Where(t => t.IsClass && t.GetCustomAttributes(attributeType, false).Any());

                if (enableCaching)
                {
                    _topicCache["topics"] = classesWithAttribute;
                }

                return classesWithAttribute;
            }

            return Enumerable.Empty<Type>();
        }
    }
}
