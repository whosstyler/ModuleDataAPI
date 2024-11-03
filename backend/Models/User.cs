using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace backend.Models
{
    public enum Role
    {
        Developer,
        Admin,
        User
    }

    public class User
    {
        [BsonId] // Maps this property to MongoDB's _id field
        [BsonRepresentation(BsonType.ObjectId)] // Ensures it's treated as an ObjectId
        public string Id { get; set; } // This maps to MongoDB's _id field
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public Role Role { get; set; }
        public string HWID { get; set; }
        public string Avatar { get; set; }
    }
}
