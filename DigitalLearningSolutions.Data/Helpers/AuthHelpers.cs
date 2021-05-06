namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.DbModels;

    public static class AuthHelpers
    {
        private static readonly TimeSpan ResetPasswordHashExpiryTime = TimeSpan.FromHours(2);

        public static bool IsStillValidAt(this ResetPassword passwordReset, DateTime dateTime)
        {
            return passwordReset.PasswordResetDateTime + ResetPasswordHashExpiryTime > dateTime;
        }
    }
}
