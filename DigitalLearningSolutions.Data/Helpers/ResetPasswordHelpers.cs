namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Models.User;

    public static class ResetPasswordHelpers
    {
        public static readonly TimeSpan ResetPasswordHashExpiryTime = TimeSpan.FromHours(2);
        public static readonly TimeSpan SetPasswordHashExpiryTime = TimeSpan.FromDays(3);

        public static bool IsStillValidAt(this ResetPassword passwordReset, DateTime dateTime, TimeSpan expiryTime)
        {
            return passwordReset.PasswordResetDateTime > dateTime;
        }

        public static IEnumerable<int> GetDistinctResetPasswordIds(this (AdminUser?, List<DelegateUser>) userAccounts)
        {
            var (adminUserIfAny, delegateUsers) = userAccounts;

            return delegateUsers
                .Select(du => (User)du)
                .Concat(new[] { adminUserIfAny })
                .Select(u => u?.ResetPasswordId)
                .Where(rPId => rPId.HasValue)
                .Select(rPId => rPId!.Value)
                .Distinct();
        }
    }
}
