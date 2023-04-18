namespace DigitalLearningSolutions.Data.Models.User
{
    using System;

    public class UserCentreDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CentreId { get; set; }
        public string? Email { get; set; }
        public DateTime? EmailVerified { get; set; }
    }
}
