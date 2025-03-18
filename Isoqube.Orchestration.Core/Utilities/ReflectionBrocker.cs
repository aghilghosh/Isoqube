using Isoqube.Orchestration.Core.Configurations.Utilities;
using Isoqube.Orchestration.Core.Data.Entities;
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

            return [];
        }

        public static IEnumerable<RegisteredTopic> GetRegisteredTopics()
        {
            Type attributeType = typeof(TopicName);
            var topics = GetTopics(true).Select(type => new
            {
                ClassName = type.Name?.Trim(),
                AttributeDescription = ((TopicName)Attribute.GetCustomAttribute(type, attributeType))?.Description?.Trim() ?? type.Name?.Trim()
            });

            return topics.Select(topic => new RegisteredTopic
            {
                CreatedOn = PlatformDateTime.Datetime,
                Description = topic.AttributeDescription?.Trim(),
                Type = topic.ClassName?.Split('`')[0]?.Trim(),
                Name = topic.ClassName?.Split('`')[0]?.Trim(),
                Version = Environment.GetEnvironmentVariable("VERSION") ?? "1.0.0"
            });
        }
    }
}
