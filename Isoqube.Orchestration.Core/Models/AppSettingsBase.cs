namespace Isoqube.Orchestration.Core.Models
{
    public class AppSettingsBase
    {
        public static RabbitMqSettings? RabbitMqSettings { get; set; }
        public static MongoDbConnection? Mongo { get; set; }
        public static List<string>? Consumers { get; set; }
    }

    public class MongoDbConnection
    {
        public string? ConnectionString { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? DatabaseName { get; set; }
    }

    public class RabbitMqSettings
    {
        public string? UserName { get; set; }
        public string? Port { get; set; }
        public string? Password { get; set; }
        public string? virtualHost { get; set; }
        public bool? EnableSsl { get; set; }
        public int? TimeoutInSeconds { get; set; }
        public bool? EnableLogging { get; set; }
        public int? PrefetchCount { get; set; }
    }

    public class VirtualHost
    {
        public string? Name { get; set; }
        public string? ManagementConsoleConnectionString { get; set; }
        public string? AdminUserPassword { get; set; }
        public bool? AdminUseruserName { get; set; }
        public ushort Port { get; set; }
        public bool? EnableLogging { get; set; }
        public int? PrefetchCount { get; set; }
    }
}