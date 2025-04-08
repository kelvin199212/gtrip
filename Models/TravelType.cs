using MongoDB.Bson;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fillow.Models
{
    public class TravelActivity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class TravelType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string TypeName { get; set; }
        public List<TravelActivity> Activities { get; set; }
    }
}
