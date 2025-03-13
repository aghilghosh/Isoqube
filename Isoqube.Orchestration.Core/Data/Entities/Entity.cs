using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Isoqube.SharedServices.Framework.Models
{
    public class Entity
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets VarVersion
        /// </summary>
        [BsonElement("Version")]
        public string? Version { get; set; } = Environment.GetEnvironmentVariable("VERSION");
    }

    public class PlatformEntity
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets CreatedOn
        /// </summary>
        [BsonElement("CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or Sets DisabledOn
        /// </summary>
        [BsonElement("DisabledOn")]
        public DateTime? DisabledOn { get; set; }

        /// <summary>
        /// Gets or Sets DeletedOn
        /// </summary>
        [BsonElement("DeletedOn")]
        public DateTime? DeletedOn { get; set; }

        /// <summary>
        /// Gets or Sets ModifiedOn
        /// </summary>
        [BsonElement("ModifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or Sets VarVersion
        /// </summary>
        [BsonElement("Version")]
        public string? Version { get; set; } = Environment.GetEnvironmentVariable("VERSION");
    }
}
