using Fillow.Models;
using Fillow.Models.partneradmin;
using MongoDB.Bson;

namespace Fillow.Services
{
    public interface ITravelPlanService
    {
        Task CreateTravelPlanAsync(TravelPlan travelPlan);
        Task DeleteTravelPlanAsync(string id);
        List<T> ExtractDynamicList<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false) where T : class, new();
        List<Exclude> ExtractDynamicListExclude<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false);
        List<Faq> ExtractDynamicListFAQ<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false);
        List<string> ExtractDynamicListHotImage<T>(BsonDocument doc, Dictionary<string, List<string>> files);
        List<string> ExtractDynamicListImage<T>(BsonDocument doc, Dictionary<string, List<string>> files);
        List<Include> ExtractDynamicListInclude<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false);
        List<Property> ExtractDynamicListProperty<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false);
        List<Models.TripItems> ExtractDynamicListTrip<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false);
        string ExtractDynamicListvideo<T>(Dictionary<string, List<string>> files);
        List<TravelType> GetActiveTravelTypes();
        List<string> GetActivitiesByType(string travelTypeId);
        Task<TravelPlan> GetTravelPlanByIdAsync(string id);
        Task<List<TravelPlan>> GetTravelPlansAsync();
        Task<List<TripViewModel>> GetTripsForUserAsync(string userId);
        TripViewModel GetTripViewModel(string tripId);
        Task InsertTripFormDataAsync(IFormCollection formData, string userId);
        Task UpdateTravelPlanAsync(string id, TravelPlan updatedPlan);

        Task UpdateTripFormDataAsync(IFormCollection formData, string userId);
    }
}