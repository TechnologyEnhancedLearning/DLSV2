namespace DigitalLearningSolutions.Data.Models.User
{
    public class UserCentreDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CentreId { get; set; }
        public string? Email { get; set; }
        public bool? EmailVerified { get; set; }
    }
}
