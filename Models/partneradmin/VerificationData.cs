using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Fillow.Models
{
    public class VerificationData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }

        public string? CompanyPhoneNumber { get; set; }
        public string? RepresentativeName { get; set; }
        public string? RepresentativePosition { get; set; }
        public string? PassportId { get; set; }
        public DateTime? IssueDate { get; set; }

        public string? personalIdPhotoPath { get; set; }
        public string? IdPhotoPath { get; set; }
        public string? GuidePhotoPath { get; set; }
        public string? BusinessLicensePath { get; set; }

        public bool? Approve { get; set; }
    }
}
