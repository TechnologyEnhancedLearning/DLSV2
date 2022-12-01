namespace DigitalLearningSolutions.Data.Models
{
    using System;
    using DigitalLearningSolutions.Data.Utilities;

    public class EmailVerificationDetails
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string EmailVerificationHash { get; set; } = null!;
        public DateTime? EmailVerified { get; set; }
        public DateTime EmailVerificationHashCreatedDate { get; set; }
        public int? CentreIdIfEmailIsForUnapprovedDelegate { get; set; }

        public bool IsEmailVerified => EmailVerified != null;

        public bool HasVerificationExpired(IClockUtility clockUtility)
        {
            return EmailVerificationHashCreatedDate <= clockUtility.UtcNow - TimeSpan.FromDays(14);
        }
    }
}
