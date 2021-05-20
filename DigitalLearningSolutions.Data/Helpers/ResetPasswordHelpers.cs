namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DbModels;
    using DigitalLearningSolutions.Data.Models.User;

    public static class ResetPasswordHelpers
    {
        private static readonly TimeSpan ResetPasswordHashExpiryTime = TimeSpan.FromHours(2);

        public static bool IsStillValidAt(this ResetPassword passwordReset, DateTime dateTime)
        {
            return passwordReset.PasswordResetDateTime + ResetPasswordHashExpiryTime > dateTime;
        }

        public static IEnumerable<int> GetDistinctResetPasswordIds(this (AdminUser?, List<DelegateUser>) userAccounts)
        {
            var (adminUserIfAny, delegateUsers) = userAccounts;

            return delegateUsers
                .Select(du => (User)du)
                .Concat(new[] { adminUserIfAny })
                .Select(u => u?.ResetPasswordId ?? (int?) null)
                .Where(rPId => rPId.HasValue)
                .Select(rPId => rPId.Value)
                .Distinct();
        }
    }
}
