using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ChatRealTime.Entities
{
    public class Messages
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }

        [BsonElement("Text")]
        [BsonRequired]
        public string? Text { get; set; }

        [BsonElement("Users")]
        public List<User>? Users { get; set; }

        [BsonElement("Sender")]
        [BsonRequired]
        public User? Sender { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Timestamp { get; set; }
    }
}