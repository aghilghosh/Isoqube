using Isoqube.Orchestration.Core.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Isoqube.Orchestration.Core.Configurations.Models
{
    public class AppSettingsBase
    {
        public static ServiceBusOptions? ServiceBusOptions { get; set; }
        public static RabbitMq? RabbitMq { get; set; }
        public static MongoDbConnection? Mongo { get; set; }
        public static Logger? Logger { get; set; }
    }

    public class MongoDbConnection
    {
        public string? ConnectionString { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? DatabaseName { get; set; }
        public string? LogCollectionName { get; set; }
        public LogLevel? LogLevel { get; set; }
    }

    public class RabbitMq
    {
        public string? UserName { get; set; }
        public string? Port { get; set; }
        public string? Password { get; set; }
        public string? VirtualHost { get; set; }
        public string? ConnectionString { get; set; }
        public bool? EnableSsl { get; set; }
        public int? TimeoutInSeconds { get; set; }
        public bool? EnableLogging { get; set; }
        public int? PrefetchCount { get; set; }
    }

    public class Logger
    {
        public bool EnableIsolatedLogging { get; set; }
        public LogProvider Provider { get; set; }
    }

    public class LogProvider
    {
        public MongoDbConnection MongoDb { get; set; }
    }
}