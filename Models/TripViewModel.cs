using Fillow.Models.partneradmin;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Fillow.Models
{
    public class TripViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? tripId { get; set; }
        public string JourneyName { get; set; }
        public string JourneyDescription { get; set; }

        public string? allEvents { get; set; }
        public string TravelType { get; set; }  // Refers to TravelType Id
        public Dictionary<string, string> Activities { get; set; }  // Maps activity IDs to their active state (true/false)
        public string DurationSelection { get; set; }
        public string LanguageSelection { get; set; }
        public string TransportationSelection { get; set; }
        public string Status { get; set; }
        public string Files { get; set; }  // JSON representation of files
        public string VideoUrl { get; set; }
        public string ScheduleList { get; set; }  // JSON representation of schedule

        public string tripDescription { get; set; }
        public int MinPeople { get; set; }
        public int MaxPeople { get; set; }
        public string DelayBookDay { get; set; }
        public List<Faq> Faq { get; set; }  // Dynamic FAQ list
        public List<TripItems> TripItems { get; set; }  // Dynamic trip item list
        public List<Include> Includes { get; set; }  // Dynamic "includes" list
        public List<Exclude> Excludes { get; set; }  // Dynamic "excludes" list
        public List<Property> Properties { get; set; }  // Dynamic property list

        public List<String> ImageList { get; set; }
        public List<String> HotImageList { get; set; }
        

        public string videoUpload {  get; set; } 
        public string ShowPrice { get; set; }
        public string AuditPrice { get; set; }
        public string ChildPrice { get; set; }
        public string HiddenAudit { get; set; }
        public string HiddenChild { get; set; }
        public string CancelAllow { get; set; }
        public string AuditNameRequire { get; set; }
        public string ChildNameRequire { get; set; }
        public string City { get; set; }
        public string LocationStart { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } 
        public string EditPageShowImageUrl   { get; set; }
}

    public class Faq
    {
        public string index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class TripItems
    {
        public string index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Url { get; set; }
    }

    public class Include
    {
        public string index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Exclude
    {
        public string index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Property
    {
        public string index { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string ImageUrl { get; set; }

        public string MapUrl { get; set; }
    }

}
