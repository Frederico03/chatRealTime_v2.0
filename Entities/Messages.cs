using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ChatRealTime.Entities
{
    public class Messages
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("message")]
        [BsonRequired]
        public required MessageText Content { get; set; }

        [BsonElement("users")]
        public List<string>? Users { get; set; }

        [BsonElement("sender")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Sender { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Timestamp { get; set; }
    }
}