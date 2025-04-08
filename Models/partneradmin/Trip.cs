using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Fillow.Models.partneradmin
{
    public class Trip
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }

        public string? JourneyName { get; set; }
        public string? JourneyDescription { get; set; }

        public int TravelType { get; set; }          // 旅遊類別 (1: 代辦服務, 2: 團體旅程, etc.)
        public bool ActivitiesVisa { get; set; } = false; // Default to false (unchecked)
        public bool ActivitiesItinerary { get; set; } = false; // Default to false (unchecked)
        public bool ActivitiesTickets { get; set; } = false; // Default to false (unchecked)
        public bool ActivitiesSpecialEvents { get; set; } = false; // Default to false (unchecked)
        public bool ActivitiesRestaurant { get; set; } = false; // Default to false (unchecked)


        public List<FAQItem> FAQItems { get; set; } = new List<FAQItem>();
        public List<TripItems> TripItems { get; set; } = new List<TripItems>();
        public List<IncludeItem> IncludeItems { get; set; } = new List<IncludeItem>();
        public List<ExcludeItem> ExcludeItems { get; set; } = new List<ExcludeItem>();
        public List<PropertyItem> PropertyItems { get; set; } = new List<PropertyItem>();
    }

    public class FAQItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class TripItems
    {
        public string Title { get; set; }
        public string ImagePath { get; set; } // Path to the uploaded image
        public string Description { get; set; }
    }

    public class IncludeItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    public class ExcludeItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class PropertyItem
    {
        public string Title { get; set; }
        public string FeaturedImagePath { get; set; } // Path to the featured image
        public string Description { get; set; }
        public string IconMapImagePath { get; set; } // Path to the icon map image
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }





}
