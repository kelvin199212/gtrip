using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
namespace Fillow.Models.partneradmin
{
	public class JourneyOrder
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? PONumber { get; set; }
		public string? UserId  { get; set; }
		public string CustomerEmail { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneNumber { get; set; }
		public string Address { get; set; }
		public string Address2 { get; set; }
		public string Gender { get; set; }
		public DateTime BirthDate { get; set; }
		public string About { get; set; }
		public string LocalAirport { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
	}

}
