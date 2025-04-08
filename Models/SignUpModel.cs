namespace Fillow.Models
{
    public class SignUpModel
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReferralCode { get; set; }
        public string UserType { get; set; } // normal or partner
        public bool AcceptTerms { get; set; }
    }
}
