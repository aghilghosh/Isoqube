using Isoqube.Orchestration.Core.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Isoqube.Orchestration.Core.Utilities
{
    public class MongoLogger : ILogger
    {
        private readonly string applicationName;
        private readonly string environmentName;
        private readonly IMongoCollection<Log> logCollection;
        private Func<string, LogLevel, bool>? filter;
        private readonly string name;

        public MongoLogger(string name, IMongoCollection<Log> collection, Func<string, LogLevel, bool> filter, string environmentName, string applicationName)
        {
            Filter = filter ?? ((category, logLevel) => true);
            this.environmentName = environmentName;
            this.applicationName = applicationName;
            this.name = name;
            logCollection = collection;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Filter(name, logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var title = formatter(state, exception);

            var exceptinon = exception?.ToString();

            var log = new Log
            {
                Message = title,
                Exception = exceptinon,
                Level = logLevel.ToString().ToUpper(),
                ApplicationName = applicationName,
                Environment = environmentName
            };

            logCollection.InsertOne(log);
        }

        private Func<string, LogLevel, bool> Filter
        {
            get { return filter; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                filter = value;
            }
        }
    }

    public static class MongoLoggerExtension
    {
        public static ILoggingBuilder AddMongoDbLogger(this ILoggingBuilder factory, MongoConfiguration configuration, string applicationName, string environmentName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            ILoggerProvider provider = new MongoLoggerProvider((n, l) => l >= configuration.MinLevel, configuration, applicationName, environmentName);

            factory.AddProvider(provider);

            return factory;
        }

        public static ILoggingBuilder AddMongoDbLogger(this ILoggingBuilder factory, Func<string, LogLevel, bool> filter, MongoConfiguration configuration, string applicationName, string environmentName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            ILoggerProvider provider = new MongoLoggerProvider(filter, configuration, applicationName, environmentName);

            factory.AddProvider(provider);

            return factory;
        }

        public static ILoggingBuilder AddMongoDbLogger(this ILoggingBuilder factory, MongoConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            ILoggerProvider provider = new MongoLoggerProvider((n, l) => l >= configuration.MinLevel, configuration, hostingEnvironment.ApplicationName, hostingEnvironment.EnvironmentName);

            factory.AddProvider(provider);

            return factory;
        }

        public static ILoggingBuilder AddMongoDbLogger(this ILoggingBuilder factory, Func<string, LogLevel, bool> filter, MongoConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            ILoggerProvider provider = new MongoLoggerProvider(filter, configuration, hostingEnvironment.ApplicationName, hostingEnvironment.EnvironmentName);

            factory.AddProvider(provider);

            return factory;
        }
    }

    public class MongoLoggerProvider : ILoggerProvider
    {
        private readonly string applicationName;
        private readonly MongoConfiguration configuration;
        private readonly string environmentName;
        private readonly Func<string, LogLevel, bool> filter;

        public MongoLoggerProvider(Func<string, LogLevel, bool> filter,
                                            MongoConfiguration configuration,
            string applicationName, string environmentName)
        {
            this.filter = filter;
            this.configuration = configuration;
            this.applicationName = applicationName;
            this.environmentName = environmentName;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new MongoLogger(categoryName, configuration.logCollection, filter, environmentName, applicationName);
        }
    }

    public class MongoConfiguration
    {
        public IMongoCollection<Log> logCollection { get; set; }
        public LogLevel MinLevel { get; set; }
    }
}
