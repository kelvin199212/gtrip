using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Fillow.Models.partneradmin
{
    public class WithdrawalRequestModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? PaymentMethod { get; set; }
        public string? UserId { get; set; } // Associated UserId

        [Required]
        [Display(Name = "帳號號碼")]
        public string AccountNumber { get; set; }

        public string? BankName { get; set; }


        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "提款金額必須大於零")]
        [Display(Name = "提款金額")]
        public decimal Amount { get; set; }

        public string? Status { get; set; }

        public DateTime? createDate { get; set; }
    }


}
