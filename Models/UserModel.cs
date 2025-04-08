using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fillow.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("FullName")]
        public string FullName { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("UserType")]
        public string UserType { get; set; }

        [BsonElement("AcceptTerms")]
        public bool AcceptTerms { get; set; }
    }
}
