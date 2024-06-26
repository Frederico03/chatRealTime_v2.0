using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ChatRealTime.Entities
{
    public class User
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        [BsonRequired]
        [StringLength(20, MinimumLength = 3)]
        public string? Username { get; set; }

        [BsonElement("email")]
        [BsonRequired]
        [StringLength(50)]
        public string? Email { get; set; }

        [BsonElement("password")]
        [BsonRequired]
        [StringLength(100, MinimumLength = 8)]
        public string? Password { get; set; }

        [BsonElement("isAvatarImageSet")]
        [BsonDefaultValue(false)]
        public bool IsAvatarImageSet { get; set; }

        [BsonElement("avatarImage")]
        [BsonDefaultValue("")]
        public string? AvatarImage { get; set; }

    }
}