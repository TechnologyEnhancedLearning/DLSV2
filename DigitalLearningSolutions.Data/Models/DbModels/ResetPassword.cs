namespace DigitalLearningSolutions.Data.Models.DbModels
{
    using System;
    using Dapper.Contrib.Extensions;

    [Table("ResetPassword")]
    public class ResetPassword
    {
        [Key]
        public int Id { get; set; }
        public string ResetPasswordHash { get; set; }
        public DateTime PasswordResetDateTime { get; set; }
    }
}
