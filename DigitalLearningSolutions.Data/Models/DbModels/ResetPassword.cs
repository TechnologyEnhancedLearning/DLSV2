namespace DigitalLearningSolutions.Data.Models.DbModels
{
    using System;

    public class ResetPassword
    {
        public int Id { get; set; }
        public string ResetPasswordHash { get; set; } = string.Empty;
        public DateTime PasswordResetDateTime { get; set; }
    }
}
