using Amazon.Runtime.Internal.Transform;
using Fillow.Models;
using Fillow.Models.partneradmin;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Rename;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fillow.Services
{
    public class TravelPlanService : ITravelPlanService
    {
        private readonly IMongoCollection<TravelPlan> _travelPlans;
        private readonly IMongoCollection<TravelType> _travelTypes;
        private readonly IMongoCollection<BsonDocument> _trip;

        public TravelPlanService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase(config["DatabaseSettings:DatabaseName"]);
            _travelPlans = database.GetCollection<TravelPlan>("TravelPlans");
            _travelTypes = database.GetCollection<TravelType>("TravelTypes");
            _trip = database.GetCollection<BsonDocument>("Trip");
        }

        public async Task<List<TravelPlan>> GetTravelPlansAsync()
        {
            return await _travelPlans.Find(travel => true).ToListAsync();
        }

        public List<TravelType> GetActiveTravelTypes()
        {
            var travelTypes = _travelTypes.Find(_ => true).ToList();
            return travelTypes.Select(t => new TravelType
            {
                Id = t.Id.ToString(),
                TypeName = t.TypeName,
                Activities = t.Activities
            }).ToList();
        }



        public List<string> GetActivitiesByType(string travelTypeId)
        {
            var travelType = _travelTypes.Find(t => t.Id.ToString() == travelTypeId).FirstOrDefault();
            return travelType?.Activities.Select(a => a.Name).ToList() ?? new List<string>();
        }
        public async Task<TravelPlan> GetTravelPlanByIdAsync(string id)
        {
            return await _travelPlans.Find(travel => travel.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateTravelPlanAsync(TravelPlan travelPlan)
        {
            await _travelPlans.InsertOneAsync(travelPlan);
        }

        public async Task UpdateTravelPlanAsync(string id, TravelPlan updatedPlan)
        {
            await _travelPlans.ReplaceOneAsync(travel => travel.Id == id, updatedPlan);
        }

        public async Task DeleteTravelPlanAsync(string id)
        {
            await _travelPlans.DeleteOneAsync(travel => travel.Id == id);
        }
        public async Task InsertTripFormDataAsync(IFormCollection formData, string userId)
        {
            // Remove entries with an empty key
            var filteredData = formData
                .Where(field => !string.IsNullOrWhiteSpace(field.Key))
                .ToDictionary(field => field.Key, field => field.Value.ToString());

            // Process files and store only their paths
            var filesData = new Dictionary<string, List<string>>();

            foreach (var file in formData.Files)
            {
                if (file.Length > 0)
                {
                    string _fileStoragePath = Path.Combine("wwwroot", "uploads", userId, "Trip", "Files");
                    if (!Directory.Exists(_fileStoragePath))
                    {
                        Directory.CreateDirectory(_fileStoragePath);
                    }

                    // Generate a unique file name
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_fileStoragePath, fileName);

                    // Save the file to the file system
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Store file path in a list under the input name (key)
                    if (!filesData.ContainsKey(file.Name))
                    {
                        filesData[file.Name] = new List<string>();
                    }
                    filesData[file.Name].Add($"/uploads/{userId}/Trip/Files/{fileName}"); // URL format for accessing the file
                }
            }

            // Add file data (file paths) to the filtered form data
            filteredData["Files"] = Newtonsoft.Json.JsonConvert.SerializeObject(filesData);

            // Convert the dictionary into a BsonDocument for MongoDB
            var tripDocument = new BsonDocument(filteredData)
            {
                { "UserId", userId },
                { "Status", "批核中" }, // Add status for review
                { "CreatedAt", DateTime.UtcNow }
            };

            // Insert into MongoDB
            await _trip.InsertOneAsync(tripDocument);
        }

        public async Task UpdateTripFormDataAsync(IFormCollection formData, string userId)
        {
            // Retrieve tripId from formData
            if (!formData.TryGetValue("tripId", out var tripIdValue) || string.IsNullOrEmpty(tripIdValue))
            {
                Console.WriteLine("tripId is missing in the form data.");
                return;
            }

            // Parse tripId into ObjectId
            ObjectId tripObjectId;
            try
            {
                tripObjectId = ObjectId.Parse(tripIdValue.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid tripId format.");
                return;
            }

            // Fetch the original document from MongoDB
            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("_id", tripObjectId),
                Builders<BsonDocument>.Filter.Eq("UserId", userId)
            );

            var originalDocument = await _trip.Find(filter).FirstOrDefaultAsync();
            if (originalDocument == null)
            {
                Console.WriteLine("No existing document found for the given tripId.");
                return;
            }

            // Prepare update definitions only for changed values
            var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();
            var updateBuilder = Builders<BsonDocument>.Update;

            if (formData.ContainsKey("hotimageRemoveList"))
            {
                var removeListJson = formData["hotimageRemoveList"].ToString();
                var removeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(removeListJson);

                if (removeList != null)
                {

                    if (removeList.Count > 0)
                    {
                        // Remove from the Files field if key exists
                        if (originalDocument.Contains("Files"))
                        {
                            var filesJson = originalDocument["Files"].AsString;
                            var filesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(filesJson) ?? new Dictionary<string, List<string>>();

                            // Check if hotimage[] exists in Files
                            if (filesDict.ContainsKey("hotimage[]"))
                            {
                                // Filter out the URLs in the "hotimage[]" array that are in removeList
                                filesDict["hotimage[]"] = filesDict["hotimage[]"]
                                                            .Where(file => !removeList.Contains(file))  // Remove matching file URLs
                                                            .ToList();

                                // If "hotimage[]" becomes empty, remove it
                                if (filesDict["hotimage[]"].Count == 0)
                                {
                                    filesDict.Remove("hotimage[]");
                                }
                            }

                            // Update the Files field with the modified file data
                            updateDefinitions.Add(updateBuilder.Set("Files", Newtonsoft.Json.JsonConvert.SerializeObject(filesDict)));
                        }
                    }
                }

            }

            if (formData.ContainsKey("imageRemoveList"))
            {
                var removeListJson = formData["imageRemoveList"].ToString();
                var removeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(removeListJson);

                if (removeList !=null)
                {
                    if (removeList.Count > 0)
                    {
                        // Remove from the Files field if key exists
                        if (originalDocument.Contains("Files"))
                        {
                            var filesJson = originalDocument["Files"].AsString;
                            var filesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(filesJson) ?? new Dictionary<string, List<string>>();

                            // Check if hotimage[] exists in Files
                            if (filesDict.ContainsKey("image[]"))
                            {
                                // Filter out the URLs in the "hotimage[]" array that are in removeList
                                filesDict["image[]"] = filesDict["image[]"]
                                                            .Where(file => !removeList.Contains(file))  // Remove matching file URLs
                                                            .ToList();

                                // If "hotimage[]" becomes empty, remove it
                                if (filesDict["image[]"].Count == 0)
                                {
                                    filesDict.Remove("image[]");
                                }
                            }

                            // Update the Files field with the modified file data
                            updateDefinitions.Add(updateBuilder.Set("Files", Newtonsoft.Json.JsonConvert.SerializeObject(filesDict)));
                        }
                    }
                }
            }
            // Handle removeList for both files and other fields
            if (formData.ContainsKey("removeList"))
            {
                var removeListJson = formData["removeList"].ToString();
                var removeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(removeListJson);

                if (removeList != null)
                {
                    if (removeList.Count > 0)
                    {
                        // Remove matching keys from the original document
                        foreach (var keyToRemove in removeList)
                        {
                            // Remove from the Files field if key exists
                            if (originalDocument.Contains("Files"))
                            {
                                var filesJson = originalDocument["Files"].AsString;
                                var filesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(filesJson) ?? new Dictionary<string, List<string>>();

                                // Check if any file in the Files field matches the key to remove
                                if (filesDict.ContainsKey(keyToRemove))
                                {
                                    // Remove the key from the Files field
                                    filesDict.Remove(keyToRemove);
                                }

                                // Update the Files field with the new value (without the removed key)
                                updateDefinitions.Add(updateBuilder.Set("Files", Newtonsoft.Json.JsonConvert.SerializeObject(filesDict)));
                            }

                            // Also remove matching key from the outer fields (regular fields)
                            if (originalDocument.Contains(keyToRemove))
                            {
                                updateDefinitions.Add(updateBuilder.Unset(keyToRemove));
                            }
                        }
                    }
                }
            }

            // Process form data for changes
            foreach (var key in formData.Keys)
            {
                var newValue = formData[key].ToString();

                // Skip empty or invalid keys
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(newValue)) continue;

                // Check if the value has changed compared to the original document
                if (originalDocument.Contains(key))
                {
                    var originalValue = originalDocument[key].ToString();

                    if (originalValue != newValue)
                    {
                        if (DateTime.TryParse(newValue, out var dateValue))
                        {
                            updateDefinitions.Add(updateBuilder.Set(key, dateValue));
                        }
                        else
                        {
                            updateDefinitions.Add(updateBuilder.Set(key, newValue));
                        }
                    }
                }
                else
                {
                    if (DateTime.TryParse(newValue, out var dateValue))
                    {
                        updateDefinitions.Add(updateBuilder.Set(key, dateValue));
                    }
                    else
                    {
                        updateDefinitions.Add(updateBuilder.Set(key, newValue));
                    }
                }
            }

            // Handle file data if new files are uploaded
            if (formData.Files.Any())
            {
                Dictionary<string, List<string>> existingFiles = new Dictionary<string, List<string>>();

                var existingFilesJson = originalDocument.Contains("Files") ? originalDocument["Files"].AsString : "{}";
                var parsedJson = Newtonsoft.Json.Linq.JObject.Parse(existingFilesJson);

                if (parsedJson != null)
                {
                    // Check if parsed JSON is valid and matches the expected dictionary structure
                    existingFiles = parsedJson.ToObject<Dictionary<string, List<string>>>();
                }

                foreach (var file in formData.Files)
                {
                    if (file.Length > 0)
                    {
                        string fileStoragePath = Path.Combine("wwwroot", "uploads", userId, "Trip", "Files");
                        if (!Directory.Exists(fileStoragePath))
                        {
                            Directory.CreateDirectory(fileStoragePath);
                        }

                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(fileStoragePath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var fileUrl = $"/uploads/{userId}/Trip/Files/{fileName}";

                        // Handle specific file inputs for `hotimage[]` and `image[]`
                        if (file.Name == "hotimage[]")
                        {
                            if (!existingFiles.ContainsKey("hotimage[]"))
                            {
                                existingFiles["hotimage[]"] = new List<string>();
                            }

                            existingFiles["hotimage[]"].Add(fileUrl);
                        }
                        else if (file.Name == "image[]")
                        {
                            if (!existingFiles.ContainsKey("image[]"))
                            {
                                existingFiles["image[]"] = new List<string>();
                            }

                            existingFiles["image[]"].Add(fileUrl);
                        }
                        else
                        {
                            // Default handling for other file inputs
                            if (!existingFiles.ContainsKey(file.Name))
                            {
                                existingFiles[file.Name] = new List<string>();
                                existingFiles[file.Name].Add(fileUrl);
                            }
                            else
                            {
                                existingFiles[file.Name][0] = fileUrl;
                            }
                        }
                    }
                }

                // Update the `Files` field with the new data
                var updatedFilesJson = Newtonsoft.Json.JsonConvert.SerializeObject(existingFiles);
                updateDefinitions.Add(updateBuilder.Set("Files", updatedFilesJson));
            }

            // Add timestamp for the last modification
            updateDefinitions.Add(updateBuilder.Set("LastModifiedAt", DateTime.UtcNow));

            // Perform the update if there are changes
            if (updateDefinitions.Count > 0)
            {
                var combinedUpdate = updateBuilder.Combine(updateDefinitions);

                try
                {
                    var result = await _trip.UpdateOneAsync(filter, combinedUpdate);
                    Console.WriteLine($"Update operation completed. ModifiedCount: {result.ModifiedCount}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred during update: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No fields to update.");
            }
        }

        public async Task<List<TripViewModel>> GetTripsForUserAsync(string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("UserId", userId);
            var tripDocuments = await _trip.Find(filter).ToListAsync();

            var trips = new List<TripViewModel>();

            foreach (var doc in tripDocuments)
            {
                // Fetch the travelTypeId from the current trip document
                var travelTypeId = doc.GetValue("travelType", "").AsString;

                // Get the TravelTypeName by looking up the TravelTypes collection using travelTypeId
                var travelType = await _travelTypes.Find(t => t.Id == travelTypeId).FirstOrDefaultAsync();

                // Create the trip view model
                var trip = new TripViewModel
                {
                    Id = doc.GetValue("_id").ToString(),
                    JourneyName = doc.GetValue("JourneyName", "").AsString,
                    TravelType = travelType?.TypeName ?? "Unknown", // If travelType is found, use the TypeName, otherwise default to "Unknown"
                    AuditPrice = doc.GetValue("auditPrice", "0").AsString + "美元",
                    AuditNameRequire = $"{doc.GetValue("minPeople", "0").AsString} - {doc.GetValue("maxPeople", "0").AsString}",
                    DurationSelection = doc.GetValue("durationSelection", "").AsString,
                    LanguageSelection = doc.GetValue("languageSelection", "").AsString,
                    Status = doc.GetValue("Status", "PendingReview").AsString,
                    EditPageShowImageUrl = GetFirstHotImage(doc.GetValue("Files", new BsonDocument()).ToString())
                };

                trips.Add(trip);
            }

            return trips;
        }

        private string GetFirstHotImage(string filesJson)
        {
            if (string.IsNullOrEmpty(filesJson)) return string.Empty;

            try
            {
                // Deserialize the JSON into a dictionary
                var files = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(filesJson);

                // Ensure that files dictionary is not null and contains the relevant key
                if (files != null)
                {
                    // Look for the first "trip-image-1" or other "trip-image" type key
                    var tripImageKey = files.Keys.FirstOrDefault(k => k.Contains("trip-image"));

                    // If the key is found, return the first URL from the list of that key
                    if (tripImageKey != null && files[tripImageKey]?.Any() == true)
                    {
                        return files[tripImageKey].FirstOrDefault();
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private int ParseInt(string value)
        {
            int result;
            return int.TryParse(value, out result) ? result : 0; // Return 0 if parsing fails
        }

        public TripViewModel GetTripViewModel(string tripId)
        {
            try
            {
                var objectId = ObjectId.Parse(tripId);
                var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                var doc = _trip.Find(filter).FirstOrDefault();

                if (doc == null)
                    throw new Exception("Trip not found");

                // Safely handle the Files field
                var files = ExtractFiles(doc);
                var tripViewModel = new TripViewModel
                {
                    Id = doc["_id"]?.AsObjectId.ToString() ?? string.Empty,
                    JourneyName = doc.GetValue("JourneyName", string.Empty).AsString ?? string.Empty,
                    JourneyDescription = doc.GetValue("JourneyDescription", string.Empty).AsString ?? string.Empty,
                    TravelType = doc.GetValue("travelType", string.Empty).AsString ?? string.Empty,
                    Activities = ExtractActivities(doc),
                    DurationSelection = doc.GetValue("durationSelection", string.Empty).AsString ?? string.Empty,
                    LanguageSelection = doc.GetValue("languageSelection", string.Empty).AsString ?? string.Empty,
                    TransportationSelection = doc.GetValue("transportationSelection", string.Empty).AsString ?? string.Empty,
                    tripDescription = doc.GetValue("tripDescription", string.Empty).AsString ?? string.Empty,
                    Status = doc.GetValue("Status", "PendingReview").AsString ?? "PendingReview",
                    VideoUrl = doc.GetValue("videoUrl", string.Empty).AsString ?? string.Empty,
                    ScheduleList = doc.GetValue("scheduleList", string.Empty).ToString() ?? string.Empty,
                    MinPeople = ParseInt(doc.GetValue("minPeople", "0").AsString),
                    MaxPeople = ParseInt(doc.GetValue("maxPeople", "0").AsString),
                    DelayBookDay = doc.GetValue("delayBookDay", "0").AsString ?? "0",
                    ShowPrice = doc.GetValue("showPrice", string.Empty).AsString ?? string.Empty,
                    AuditPrice = doc.GetValue("auditPrice", "0").AsString ?? "0",
                    ChildPrice = doc.GetValue("childPrice", "0").AsString ?? "0",
                    HiddenAudit = doc.GetValue("hiddenAudit", "off").AsString ?? "off",
                    HiddenChild = doc.GetValue("hiddenChild", "off").AsString ?? "off",
                    CancelAllow = doc.GetValue("cancelAllow", "off").AsString ?? "off",
                    AuditNameRequire = doc.GetValue("AuditNameRequire", "off").AsString ?? "off",
                    ChildNameRequire = doc.GetValue("childNameRequire", "off").AsString ?? "off",
                    City = doc.GetValue("city", string.Empty).AsString ?? string.Empty,
                    LocationStart = doc.GetValue("locationStart", string.Empty).AsString ?? string.Empty,
                    CreatedAt = doc.GetValue("CreatedAt", BsonNull.Value) == BsonNull.Value ? DateTime.MinValue : doc.GetValue("CreatedAt", BsonNull.Value).ToUniversalTime(),
                    UserId = doc.GetValue("UserId", string.Empty).AsString ?? string.Empty,
                    EditPageShowImageUrl = GetFirstImageUrl(files),
                    Faq = ExtractDynamicListFAQ<Faq>(doc, "faq", files),
                    TripItems = ExtractDynamicListTrip<Fillow.Models.TripItems>(doc, "trip", files),
                    Includes = ExtractDynamicListInclude<Include>(doc, "include", files),
                    Excludes = ExtractDynamicListExclude<Exclude>(doc, "exclude", files),
                    Properties = ExtractDynamicListProperty<Property>(doc, "property", files),
                    videoUpload = ExtractDynamicListvideo<string>(files),
                    ImageList = ExtractDynamicListImage<string>(doc, files),
                    HotImageList = ExtractDynamicListHotImage<string>(doc, files)
                };

                return tripViewModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return null;
        }
        private Dictionary<string, List<string>> ExtractFiles(BsonDocument doc)
        {
            var result = new Dictionary<string, List<string>>();

            if (doc.TryGetValue("Files", out var filesValue))
            {
                try
                {
                    // Check if it's a BsonDocument
                    if (filesValue is BsonDocument bsonDocument)
                    {
                        result = BsonDocumentToDictionary(bsonDocument);
                    }
                    // Check if it's a stringified JSON
                    else if (filesValue.IsString)
                    {
                        result = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(filesValue.AsString);
                    }
                    // Handle if it's a BsonArray
                    else if (filesValue is BsonArray bsonArray)
                    {
                        foreach (var item in bsonArray)
                        {
                            if (item is BsonDocument docItem)
                            {
                                foreach (var key in docItem.Names)
                                {
                                    var values = docItem[key].AsBsonArray.Select(x => x.AsString).ToList();
                                    result[key] = values;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing Files field: {ex.Message}");
                }
            }

            return result;
        }

        public List<Faq> ExtractDynamicListFAQ<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false)
        {
            var result = new List<Faq>();
            var index = 0;
            foreach (var element in doc.Elements)
            {
                if (element.Name.Contains(key + "-title", StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
            }

            for (int i = 1; i <= index; i++)
            {
                var newFaq = new Faq();
                foreach (var element in doc.Elements)
                {
                    if (element.Name.Contains(key + "-title-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.index = i.ToString();
                        newFaq.Title = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-description-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Description = element.Value.ToString();
                    }
                }
                result.Add(newFaq);
            }

            return result;
        }
        public List<Include> ExtractDynamicListInclude<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false)
        {
            var result = new List<Include>();
            var index = 0;
            foreach (var element in doc.Elements)
            {
                if (element.Name.Contains(key + "-title", StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
            }

            for (int i = 1; i <= index; i++)
            {
                var newFaq = new Include();
                foreach (var element in doc.Elements)
                {
                    if (element.Name.Contains(key + "-title-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.index = i.ToString();
                        newFaq.Title = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-description-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Description = element.Value.ToString();
                    }
                }
                result.Add(newFaq);
            }

            return result;
        }

        public List<Exclude> ExtractDynamicListExclude<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false)
        {
            var result = new List<Exclude>();
            var index = 0;
            foreach (var element in doc.Elements)
            {
                if (element.Name.Contains(key + "-title", StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
            }

            for (int i = 1; i <= index; i++)
            {
                var newFaq = new Exclude();
                foreach (var element in doc.Elements)
                {
                    if (element.Name.Contains(key + "-title-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.index = i.ToString();
                        newFaq.Title = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-description-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Description = element.Value.ToString();
                    }
                }
                result.Add(newFaq);
            }

            return result;
        }

        public List<Fillow.Models.TripItems> ExtractDynamicListTrip<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false)
        {
            var result = new List<Fillow.Models.TripItems>();
            var index = 0;
            foreach (var element in doc.Elements)
            {
                if (element.Name.Contains(key + "-title", StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
            }

            for (int i = 1; i <= index; i++)
            {
                var newFaq = new Fillow.Models.TripItems();
                foreach (var element in doc.Elements)
                {
                    if (element.Name.Contains(key + "-title-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.index = i.ToString();
                        newFaq.Title = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-description-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Description = element.Value.ToString();
                    }
                    foreach (var fileKey in files.Keys)
                    {
                        if (fileKey.Contains(key + "-image-" + i, StringComparison.OrdinalIgnoreCase))
                        {
                            if (files.TryGetValue(fileKey, out var fileValues) && fileValues.Count > 0)
                            {
                                newFaq.Url = fileValues[0];
                            }
                        }
                    }

                }
                result.Add(newFaq);
            }

            return result;
        }
        public List<Property> ExtractDynamicListProperty<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false)
        {
            var result = new List<Property>();
            var index = 0;
            foreach (var element in doc.Elements)
            {
                if (element.Name.Contains(key + "-title", StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
            }

            for (int i = 1; i <= index; i++)
            {
                var newFaq = new Property();
                foreach (var element in doc.Elements)
                {
                    if (element.Name.Contains(key + "-title-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.index = i.ToString();
                        newFaq.Title = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-description-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Description = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-lat-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Latitude = element.Value.ToString();
                    }
                    if (element.Name.Contains(key + "-long-" + i, StringComparison.OrdinalIgnoreCase))
                    {
                        newFaq.Longitude = element.Value.ToString();
                    }
                    foreach (var fileKey in files.Keys)
                    {
                        if (fileKey.Contains(key + "-image2-" + i, StringComparison.OrdinalIgnoreCase))
                        {
                            if (files.TryGetValue(fileKey, out var fileValues) && fileValues.Count > 0)
                            {
                                newFaq.ImageUrl = fileValues[0];
                            }
                        }

                        if (fileKey.Contains(key + "-image3-" + i, StringComparison.OrdinalIgnoreCase))
                        {
                            if (files.TryGetValue(fileKey, out var fileValues) && fileValues.Count > 0)
                            {
                                newFaq.MapUrl = fileValues[0];
                            }
                        }
                    }

                }
                result.Add(newFaq);
            }

            return result;
        }

        public string ExtractDynamicListvideo<T>(Dictionary<string, List<string>> files)
        {
            string result = "";
            var newFaq = new Property();

            foreach (var fileKey in files.Keys)
            {
                if (fileKey.Contains("video-upload", StringComparison.OrdinalIgnoreCase))
                {
                    if (files.TryGetValue(fileKey, out var fileValues) && fileValues.Count > 0)
                    {
                        result = fileValues[0];
                    }
                }
            }


            return result;
        }

        public List<string> ExtractDynamicListImage<T>(BsonDocument doc, Dictionary<string, List<string>> files)
        {
            var result = new List<string>();
            var key = "image[]";


            foreach (var fileKey in files.Keys)
            {
                if (fileKey == key)
                {
                    if (files.TryGetValue(fileKey, out var fileValues) && fileValues.Count > 0)
                    {
                        foreach (var fileValue in fileValues)
                        {
                            result.Add(fileValue);
                        }
                    }
                }
            }


            return result;
        }

        public List<string> ExtractDynamicListHotImage<T>(BsonDocument doc, Dictionary<string, List<string>> files)
        {
            var result = new List<string>();
            var key = "hotimage[]";


            foreach (var fileKey in files.Keys)
            {
                if (fileKey == key)
                {
                    if (files.TryGetValue(fileKey, out var fileValues) && fileValues.Count > 0)
                    {
                        foreach (var fileValue in fileValues)
                        {
                            result.Add(fileValue);
                        }
                    }
                }
            }


            return result;
        }

        public List<T> ExtractDynamicList<T>(BsonDocument doc, string key, Dictionary<string, List<string>> files, bool includeFiles = false)
            where T : class, new()
        {
            var result = new List<T>(); // Initialize the result list

            // Loop through the BsonDocument elements
            foreach (var element in doc.Elements)
            {
                var obj = new T(); // Create a new instance of the object

                // Iterate through the fileKeys in the dictionary
                if (element.Name.Contains("", StringComparison.OrdinalIgnoreCase))
                {
                    if (element.Name.Contains("title", StringComparison.OrdinalIgnoreCase))
                    {
                        if (obj is Fillow.Models.TripItems a)
                        {
                            // Assuming you want to set Title based on the fileKey
                            a.Title = element.Value.ToString();
                        }
                    }
                    else if (element.Name.Contains("description", StringComparison.OrdinalIgnoreCase))
                    {
                        if (obj is Fillow.Models.TripItems a)
                        {
                            // Assuming you want to set Description based on the fileKey
                            a.Description = element.Value.ToString();
                        }
                    }
                    else if (element.Name.Contains("Image", StringComparison.OrdinalIgnoreCase))
                    {
                        if (obj is Fillow.Models.TripItems a)
                        {

                            a.Url = element.Value.ToString();

                        }
                    }


                    // Add the populated object to the result list
                    result.Add(obj);
                }
            }

            return result; // Return the list of populated objects
        }

        private Dictionary<string, string> ExtractActivities(BsonDocument doc)
        {
            return doc.Elements
                .Where(e => e.Name.StartsWith("activities_"))
                .ToDictionary(
                    e => e.Name.Replace("activities_", ""),
                    e => e.Value.AsString);
        }

        private Dictionary<string, List<string>> BsonDocumentToDictionary(BsonDocument bsonDocument)
        {
            var result = new Dictionary<string, List<string>>();

            foreach (var element in bsonDocument.Elements)
            {
                var key = element.Name;
                var value = element.Value;

                try
                {
                    if (value.IsBsonArray)
                    {
                        // Convert BsonArray to List<string>
                        var list = value.AsBsonArray.Select(v => v.ToString()).ToList();
                        result[key] = list;
                    }
                    else if (value.IsString)
                    {
                        // Handle single string as a list with one item
                        result[key] = new List<string> { value.AsString };
                    }
                    else if (value.IsBsonDocument)
                    {
                        // Recursively process nested BsonDocuments as strings
                        var nestedDoc = BsonDocumentToDictionary(value.AsBsonDocument);
                        result[key] = nestedDoc.SelectMany(kv => kv.Value).ToList();
                    }
                    else
                    {
                        // Handle other BSON types by converting them to strings
                        result[key] = new List<string> { value.ToString() };
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing key '{key}': {ex.Message}");
                }
            }

            return result;
        }


        private List<T> ExtractDynamicList<T>(BsonDocument doc, string prefix, Dictionary<string, string> files, bool includeLatLong = false) where T : class, new()
        {
            var result = new List<T>();

            foreach (var element in doc.Elements.Where(e => e.Name.StartsWith(prefix + "-")))
            {
                var parts = element.Name.Split('-');
                if (parts.Length < 3) continue;

                var index = parts[2];
                var property = result.FirstOrDefault(p => p.GetType().GetProperty("Title")?.GetValue(p)?.ToString() == index);

                if (property == null)
                {
                    property = new T();
                    result.Add(property);
                }

                var propName = parts[1];
                var value = element.Value.AsString;
                property.GetType().GetProperty(propName)?.SetValue(property, value);

                if (typeof(T) == typeof(Property) && includeLatLong)
                {
                    property.GetType().GetProperty("Latitude")?.SetValue(property, doc.GetValue($"{prefix}-lat-{index}", "").AsString);
                    property.GetType().GetProperty("Longitude")?.SetValue(property, doc.GetValue($"{prefix}-long-{index}", "").AsString);
                }

                if (files.ContainsKey($"{prefix}-image-{index}"))
                {
                    property.GetType().GetProperty("Url")?.SetValue(property, files[$"{prefix}-image-{index}"]);
                }
            }

            return result;
        }



        private string GetFirstImageUrl(Dictionary<string, List<string>> files)
        {
            if (files == null || !files.Any())
                return string.Empty;

            foreach (var key in files.Keys)
            {
                if (key.ToLower().Contains("image")) // Look for keys related to images
                {
                    var images = files[key];
                    if (images != null && images.Any())
                    {
                        return images.First(); // Return the first image URL found
                    }
                }
            }

            return string.Empty; // Return an empty string if no images are found
        }



    }

}
