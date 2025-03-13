using Isoqube.Orchestration.Core.Configurations.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Isoqube.Orchestration.Core.Data
{
    public class Log
    {
        public Log() => CreateOn = PlatformDateTime.Datetime;

        [BsonId]
        public ObjectId Id { get; set; }
        public DateTimeOffset CreateOn { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public string? Level { get; set; }
        public string? ApplicationName { get; set; }
        public string? Environment { get; set; }
    }
}
