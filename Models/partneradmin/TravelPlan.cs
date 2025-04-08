using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fillow.Models.partneradmin
{
    public class TravelPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int RecommendedParticipants { get; set; }
        public int DurationInDays { get; set; }
        public string Language { get; set; }
    }
}
