namespace DigitalLearningSolutions.Data.Models.Email
{
    public class Recipient
    {
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool Owner { get; set; }
        public bool Sender { get; set; }
    }
}
