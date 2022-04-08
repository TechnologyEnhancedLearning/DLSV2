// ReSharper disable InconsistentNaming

namespace DigitalLearningSolutions.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using FluentMigrator;
    using Microsoft.Data.SqlClient;

    [Migration(202204071530, TransactionBehavior.None)]
    public class PopulateUsersTableFromAccountsTables : Migration
    {
        public override void Up()
        {
            IDbConnection connection = new SqlConnection(ConnectionString);

            var options = new TransactionOptions
            {
                Timeout = new TimeSpan(0, 15, 0),
            };
            using var transactionScope = new TransactionScope(TransactionScopeOption.Required, options);

            // 1. Delete from Users (this should be empty)
            connection.Execute("Delete Users");

            // 2. Copy AdminAccounts to Users table
            connection.Execute(
                @"INSERT INTO dbo.Users (
                    PrimaryEmail,
                    PasswordHash,
                    FirstName,
                    LastName,
                    JobGroupID,
                    ProfessionalRegistrationNumber,
                    ProfileImage,
                    Active,
                    ResetPasswordID,
                    TermsAgreed,
                    FailedLoginCount,
                    HasBeenPromptedForPrn,
                    LearningHubAuthId,
                    HasDismissedLhLoginWarning,
                    EmailVerified
                )
                SELECT Email,
                    Password_deprecated,
                    Forename_deprecated,
                    Surname_deprecated,
                    10,
                    NULL,
                    ProfileImage_deprecated,
                    Active,
                    ResetPasswordID_deprecated,
                    TCAgreed_deprecated,
                    FailedLoginCount_deprecated,
                    0,
                    NULL,
                    0,
                    GETUTCDATE()
                    FROM AdminAccounts"
            );

            // 3. Update AdminAccounts to reference Users.ID
            connection.Execute(
                @"UPDATE AdminAccounts
                SET UserID = (SELECT ID FROM Users WHERE Email = PrimaryEmail)"
            );

            // 4. DelegateAccount

            // Start with just delegate accounts with emails for simplicity
            var delegateAccounts =
                connection.Query<DelegateAccount>("SELECT * FROM DelegateAccounts")
                    .ToList();

            var delegateAccountsWithEmails = delegateAccounts.Where(da => !string.IsNullOrWhiteSpace(da.Email));

            var duplicateDelegateAccountsAtCentre = new List<DelegateAccount>();

            var delegateAccountsGroupedByEmail = delegateAccountsWithEmails.GroupBy(da => da.Email);

            foreach (var emailGroup in delegateAccountsGroupedByEmail)
            {
                var delegateAccountsGroupedByCentre = emailGroup.GroupBy(da => da.CentreId);

                foreach (var centreGroup in delegateAccountsGroupedByCentre)
                {
                    var firstAtCentre = centreGroup.First();

                    var existingUserWithEmail = connection.QuerySingleOrDefault<User>(
                            "SELECT * FROM Users WHERE PrimaryEmail = @email",
                            new { email = firstAtCentre.Email }
                        );

                    if (existingUserWithEmail != null)
                    {
                        UpdateExistingUserWithDelegateDetails(
                            connection,
                            existingUserWithEmail,
                            firstAtCentre,
                            emailGroup
                        );
                    }
                    else
                    {
                        InsertNewUserForDelegateAccount(connection, firstAtCentre);
                    }

                    var othersAtCentre = centreGroup.Skip(1);

                    duplicateDelegateAccountsAtCentre.AddRange(othersAtCentre);
                }
            }

            var delegateAccountsWithoutEmails = delegateAccounts.Where(da => string.IsNullOrWhiteSpace(da.Email));

            foreach (var delegateAccount in delegateAccountsWithoutEmails)
            {
                InsertNewUserForDelegateAccount(connection, delegateAccount);
            }

            foreach (var delegateAccount in duplicateDelegateAccountsAtCentre)
            {
                InsertNewUserForDelegateAccount(connection, delegateAccount, true);
            }

            transactionScope.Complete();
        }

        private static void UpdateDelegateAccountUserIdAndDetailsLastChecked(
            IDbConnection connection,
            int userId,
            int delegateAccountId,
            DateTime? detailsLastChecked
        )
        {
            connection.Execute(
                @"UPDATE DelegateAccounts
                            SET
                                UserID = @userId,
                                DetailsLastChecked = @detailsLastChecked
                            WHERE ID = @delegateAccountId",
                new
                {
                    userId,
                    delegateAccountId,
                    detailsLastChecked,
                }
            );
        }

        public override void Down()
        {
            // empty for now, will restore snapshot
        }

        private static void UpdateExistingUserWithDelegateDetails(
            IDbConnection connection,
            User existingUserWithEmail,
            DelegateAccount delegateAccount,
            IEnumerable<DelegateAccount> delegateAccountsWithMatchingEmail
        )
        {
            connection.Execute(
                @"UPDATE Users
                            SET
                                PasswordHash = @passwordHash,
                                FirstName = @firstName,
                                LastName = @lastName,
                                JobGroupID = @jobGroupId,
                                ProfessionalRegistrationNumber = @professionalRegistrationNumber,
                                ProfileImage = @profileImage,
                                Active = @active,
                                ResetPasswordID = @resetPasswordId,
                                HasBeenPromptedForPrn = @hasBeenPromptedForPrn,
                                LearningHubAuthId = @learningHubAuthId,
                                HasDismissedLhLoginWarning = @hasDismissedLhLoginWarning,
                                EmailVerified = @emailVerified
                            WHERE ID = @userId",
                new
                {
                    userId = existingUserWithEmail.Id,
                    passwordHash = string.IsNullOrEmpty(existingUserWithEmail.PasswordHash)
                        ? delegateAccount.OldPassword ?? ""
                        : existingUserWithEmail.PasswordHash,
                    firstName = string.IsNullOrEmpty(existingUserWithEmail.FirstName)
                        ? delegateAccount.FirstName_deprecated ?? ""
                        : existingUserWithEmail.FirstName,
                    lastName = string.IsNullOrEmpty(existingUserWithEmail.LastName)
                        ? delegateAccount.LastName_deprecated
                        : existingUserWithEmail.LastName,
                    jobGroupId = existingUserWithEmail.JobGroupId == 10
                        ? delegateAccount.JobGroupId_deprecated
                        : existingUserWithEmail.JobGroupId,
                    professionalRegistrationNumber =
                        string.IsNullOrEmpty(existingUserWithEmail.ProfessionalRegistrationNumber)
                            ? delegateAccount.ProfessionalRegistrationNumber_deprecated
                            : existingUserWithEmail.ProfessionalRegistrationNumber,
                    profileImage = existingUserWithEmail.ProfileImage ?? delegateAccount.ProfileImage_deprecated,
                    active = !existingUserWithEmail.Active ? delegateAccount.Active : existingUserWithEmail.Active,
                    resetPasswordId = existingUserWithEmail.ResetPasswordId ??
                                      delegateAccount.ResetPasswordId_deprecated,
                    hasBeenPromptedForPrn = !existingUserWithEmail.HasBeenPromptedForPrn
                        ? delegateAccount.HasBeenPromptedForPrn_deprecated
                        : existingUserWithEmail.HasBeenPromptedForPrn,
                    learningHubAuthId = existingUserWithEmail.LearningHubAuthId ??
                                        delegateAccount.LearningHubAuthId_deprecated,
                    hasDismissedLhLoginWarning = !existingUserWithEmail.HasDismissedLhLoginWarning
                        ? delegateAccount.HasDismissedLhLoginWarning_deprecated
                        : existingUserWithEmail.HasDismissedLhLoginWarning,
                    emailVerified = DateTime.UtcNow,
                }
            );

            UpdateDelegateAccountUserIdAndDetailsLastChecked(
                connection,
                existingUserWithEmail.Id,
                delegateAccount.Id,
                GetDetailsLastCheckedValue(
                    connection,
                    existingUserWithEmail.Id,
                    delegateAccount,
                    delegateAccountsWithMatchingEmail
                )
            );
        }

        private static void InsertNewUserForDelegateAccount(IDbConnection connection, DelegateAccount delegateAccount, bool isDuplicateAtCentre = false)
        {
            var userId = connection.QuerySingle<int>(
                @"INSERT INTO Users
                            (
                                PrimaryEmail,
                                PasswordHash,
                                FirstName,
                                LastName,
                                JobGroupID,
                                ProfessionalRegistrationNumber,
                                ProfileImage,
                                Active,
                                ResetPasswordID,
                                TermsAgreed,
                                FailedLoginCount,
                                HasBeenPromptedForPrn,
                                LearningHubAuthId,
                                HasDismissedLhLoginWarning,
                                EmailVerified
                            )
                            OUTPUT Inserted.ID
                            VALUES
                            (
                                @primaryEmail,
                                @passwordHash,
                                @firstName,
                                @lastName,
                                @jobGroupID,
                                @professionalRegistrationNumber,
                                @profileImage,
                                @active,
                                @resetPasswordID,
                                NULL,
                                0,
                                @hasBeenPromptedForPrn,
                                @learningHubAuthId,
                                @hasDismissedLhLoginWarning,
                                @emailVerified
                            )",
                new
                {
                    primaryEmail = string.IsNullOrWhiteSpace(delegateAccount.Email) || isDuplicateAtCentre
                        ? Guid.NewGuid().ToString()
                        : delegateAccount.Email,
                    passwordHash = delegateAccount.OldPassword ?? "",
                    firstName = delegateAccount.FirstName_deprecated ?? "",
                    lastName = delegateAccount.LastName_deprecated,
                    jobGroupId = delegateAccount.JobGroupId_deprecated,
                    professionalRegistrationNumber = delegateAccount.ProfessionalRegistrationNumber_deprecated,
                    profileImage = delegateAccount.ProfileImage_deprecated,
                    active = delegateAccount.Active,
                    resetPasswordId = delegateAccount.ResetPasswordId_deprecated,
                    hasBeenPromptedForPrn = delegateAccount.HasBeenPromptedForPrn_deprecated,
                    learningHubAuthId = delegateAccount.LearningHubAuthId_deprecated,
                    hasDismissedLhLoginWarning = delegateAccount.HasDismissedLhLoginWarning_deprecated,
                    emailVerified = DateTime.UtcNow,
                }
            );

            UpdateDelegateAccountUserIdAndDetailsLastChecked(connection, userId, delegateAccount.Id, DateTime.UtcNow);
        }

        private static DateTime? GetDetailsLastCheckedValue(
            IDbConnection connection,
            int existingUserId,
            DelegateAccount currentDelegateAccount,
            IEnumerable<DelegateAccount> delegateAccountsWithMatchingEmail
        )
        {
            var existingUser = connection.QuerySingleOrDefault<User>(
                "SELECT * FROM Users WHERE ID = @existingUserId",
                new { existingUserId }
            );

            if (currentDelegateAccount.FirstName_deprecated == existingUser.FirstName &&
                currentDelegateAccount.LastName_deprecated == existingUser.LastName &&
                currentDelegateAccount.ProfileImage_deprecated == existingUser.ProfileImage &&
                delegateAccountsWithMatchingEmail.Select(da => da.JobGroupId_deprecated).Distinct().Count() < 2)
            {
                return DateTime.UtcNow;
            }

            return null;
        }

        private class DelegateAccount
        {
            public int Id { get; set; }
            public bool Active { get; set; }
            public int CentreId { get; set; }
            public string? FirstName_deprecated { get; set; }
            public string LastName_deprecated { get; set; }
            public int JobGroupId_deprecated { get; set; }
            public string? Email { get; set; }
            public string? OldPassword { get; set; }
            public byte[]? ProfileImage_deprecated { get; set; }
            public int? ResetPasswordId_deprecated { get; set; }
            public bool HasBeenPromptedForPrn_deprecated { get; set; }
            public string? ProfessionalRegistrationNumber_deprecated { get; set; }
            public int? LearningHubAuthId_deprecated { get; set; }
            public bool HasDismissedLhLoginWarning_deprecated { get; set; }
        }

        private class User
        {
            public int Id { get; set; }
            public string PasswordHash { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int JobGroupId { get; set; }
            public string? ProfessionalRegistrationNumber { get; set; }
            public byte[]? ProfileImage { get; set; }
            public bool Active { get; set; }
            public int? ResetPasswordId { get; set; }
            public bool HasBeenPromptedForPrn { get; set; }
            public int? LearningHubAuthId { get; set; }
            public bool HasDismissedLhLoginWarning { get; set; }
        }
    }
}
