using MongoDB.Bson.Serialization.Attributes;

namespace ChatRealTime.Entities
{
    public class MessageText
    {
        [BsonElement("text")]
        public string? Text { get; set; }
    }
}