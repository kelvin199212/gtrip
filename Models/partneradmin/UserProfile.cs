namespace Fillow.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


public class UserProfile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? UserId { get; set; }
    public string UserName { get; set; } 
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string About { get; set; }
    public string LocalAirport { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public string? ProfilePicturePath { get; set; } // Store file path instead of byte array
    
    public string? VideoIntroductionPath { get; set; }
}
