using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fillow.Models.partneradmin
{
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Location { get; set; }

        public DateTime? StartDate { get; set; }

        public string? userId { get; set; }
        public bool? EnableStartTime { get; set; }
        public string? StartTime { get; set; }

        public int? Duration { get; set; }

        public string? Repeat { get; set; }

        public List<string>? Days { get; set; }

        public DateTime? Until { get; set; }
    }
}
