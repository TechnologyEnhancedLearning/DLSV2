namespace DigitalLearningSolutions.Data.Models.DbModels
{
    using System;

    public class ResetPassword
    {
        public int Id { get; set; }
        public string ResetPasswordHash { get; set; }
        public DateTime PasswordResetDateTime { get; set; }
        public DateTime? ResetExpiryDateTime { get; set; }
    }
}
