// ReSharper disable InconsistentNaming

namespace DigitalLearningSolutions.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Migrations.Properties;
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
            connection.Execute("DELETE Users");

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
                    EmailVerified,
                    DetailsLastChecked
                )
                SELECT
                    Email,
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
                    GETUTCDATE(),
                    CASE WHEN TRIM(Email) IS NOT NULL AND TRIM(Email) <> '' THEN GETUTCDATE() ELSE NULL END
                    FROM AdminAccounts"
            );

            // 3. Update AdminAccounts to reference Users.ID
            connection.Execute(
                @"UPDATE AdminAccounts
                    SET
                        UserID = (SELECT ID FROM Users WHERE Email = PrimaryEmail),
                        Email = NULL"
            );

            // 4. DelegateAccount

            // Transfer all delegates with unique emails not already in the Users table
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
                    EmailVerified,
                    DetailsLastChecked
                )
                SELECT Email,
                    COALESCE(OldPassword, ''),
                    COALESCE(FirstName_deprecated, ''),
                    COALESCE(LastName_deprecated, ''),
                    JobGroupID_deprecated,
                    ProfessionalRegistrationNumber_deprecated,
                    ProfileImage_deprecated,
                    Active,
                    ResetPasswordID_deprecated,
                    NULL,
                    0,
                    HasBeenPromptedForPrn_deprecated,
                    LearningHubAuthID_deprecated,
                    HasDismissedLhLoginWarning_deprecated,
                    GETUTCDATE(),
                    GETUTCDATE()
                    FROM DelegateAccounts
                    WHERE Email IN (
                        SELECT Email FROM DelegateAccounts
                        WHERE Email IS NOT NULL AND TRIM(Email) IS NOT NULL AND TRIM(Email) <> ''
                        GROUP BY Email
                        HAVING COUNT(*) = 1
                        EXCEPT
                        SELECT PrimaryEmail FROM Users)"
            );

            // Link all these User records we just created to the DelegateAccounts.
            LinkDelegateAccountsToTheirUserRecords(connection);

            // Get the rest of the delegate accounts we've not resolved yet
            var delegateAccounts =
                connection.Query<DelegateAccount>("SELECT * FROM DelegateAccounts WHERE UserId IS NULL");

            var delegateAccountsGroupedByEmail = delegateAccounts.GroupBy(da => da.Email);

            foreach (var emailGroup in delegateAccountsGroupedByEmail)
            {
                if (!string.IsNullOrWhiteSpace(emailGroup.Key))
                {
                    var delegatesToAttemptToLink = new List<DelegateAccount>();
                    var delegateAccountsGroupedByCentre = emailGroup.GroupBy(da => da.CentreId);

                    foreach (var centreGroup in delegateAccountsGroupedByCentre)
                    {
                        delegatesToAttemptToLink.Add(centreGroup.First());

                        // All duplicate emails at a centre need new User accounts.
                        // If there are more than one delegate accounts with the same email at a centre
                        // we only want to keep the email on the first. The rest get reset to having a User with a guid email
                        var othersAtCentre = centreGroup.Skip(1);

                        foreach (var delegateAccount in othersAtCentre)
                        {
                            // Insert a new User with Guid email
                            InsertNewUserForDelegateAccount(connection, delegateAccount, true);
                        }
                    }

                    // Since the job group is only on delegate accounts before this migration, we just compare them all here
                    // before we attempt to match up any accounts
                    var allJobGroupsMatch =
                        delegatesToAttemptToLink.Select(da => da.JobGroupId_deprecated).Distinct().Count() < 2;
                    foreach (var delegateAccount in delegatesToAttemptToLink)
                    {
                        // Check for existing user with email
                        var existingUserWithEmail = connection.QuerySingleOrDefault<User>(
                            "SELECT * FROM Users WHERE PrimaryEmail = @email",
                            new { email = emailGroup.Key }
                        );

                        if (existingUserWithEmail != null)
                        {
                            // If we find a user, we update any default data on that user
                            UpdateExistingUserDefaultValuesWithDelegateDetails(
                                connection,
                                existingUserWithEmail,
                                delegateAccount,
                                allJobGroupsMatch
                            );
                        }
                        else
                        {
                            // Otherwise we create a new user with the email address
                            InsertNewUserForDelegateAccount(connection, delegateAccount, false);
                        }
                    }
                }
                else
                {
                    // If we don't have a valid email we create new users for each with a guid email
                    foreach (var delegateAccount in emailGroup)
                    {
                        InsertNewUserForDelegateAccount(connection, delegateAccount, true);
                    }
                }
            }

            // At the end we link all the unlinked accounts with emails to the matching User record.
            // All ones with invalid emails were linked when we created new User records for them.
            LinkDelegateAccountsToTheirUserRecords(connection);

            transactionScope.Complete();
        }

        public override void Down()
        {
            Execute.Sql(Resources.UAR_859_PopulateUsersTableFromAccountsTables_DOWN);
        }

        private static void LinkDelegateAccountsToTheirUserRecords(IDbConnection connection)
        {
            connection.Execute(
                @"UPDATE DelegateAccounts
                    SET
                        UserID = (SELECT ID FROM Users WHERE Email = PrimaryEmail),
                        Email = NULL,
                        CentreSpecificDetailsLastChecked = GETUTCDATE()
                    WHERE UserID IS NULL AND
                        Email IS NOT NULL AND TRIM(Email) IS NOT NULL AND TRIM(Email) <> ''"
            );
        }

        private static void UpdateExistingUserDefaultValuesWithDelegateDetails(
            IDbConnection connection,
            User existingUserWithEmail,
            DelegateAccount delegateAccount,
            bool allJobGroupsMatch
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
                                DetailsLastChecked = CASE WHEN @detailsMatched = 0 OR DetailsLastChecked IS NULL THEN NULL ELSE GETUTCDATE() END
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
                    active = existingUserWithEmail.Active || delegateAccount.Active,
                    resetPasswordId = existingUserWithEmail.ResetPasswordId ??
                                      delegateAccount.ResetPasswordId_deprecated,
                    hasBeenPromptedForPrn = existingUserWithEmail.HasBeenPromptedForPrn ||
                                            delegateAccount.HasBeenPromptedForPrn_deprecated,
                    learningHubAuthId = existingUserWithEmail.LearningHubAuthId ??
                                        delegateAccount.LearningHubAuthId_deprecated,
                    hasDismissedLhLoginWarning = existingUserWithEmail.HasDismissedLhLoginWarning ||
                                                 delegateAccount.HasDismissedLhLoginWarning_deprecated,
                    detailsMatched = DoesDelegateAccountMatchExistingUser(delegateAccount, existingUserWithEmail) && allJobGroupsMatch
                }
            );
        }

        private static bool DoesDelegateAccountMatchExistingUser(DelegateAccount delegateAccount, User existingUser)
        {
            var firstNamesMatch = delegateAccount.FirstName_deprecated == existingUser.FirstName ||
                                  string.IsNullOrWhiteSpace(existingUser.FirstName) ||
                                  string.IsNullOrWhiteSpace(delegateAccount.FirstName_deprecated);

            var lastNamesMatch = delegateAccount.LastName_deprecated == existingUser.LastName ||
                                 string.IsNullOrWhiteSpace(existingUser.LastName) ||
                                 string.IsNullOrWhiteSpace(delegateAccount.LastName_deprecated);

            var prnMatch = delegateAccount.ProfessionalRegistrationNumber_deprecated == existingUser.ProfessionalRegistrationNumber ||
                           string.IsNullOrWhiteSpace(existingUser.ProfessionalRegistrationNumber) ||
                           string.IsNullOrWhiteSpace(delegateAccount.ProfessionalRegistrationNumber_deprecated);

            var profileImageMatch = existingUser.ProfileImage == null ||
                                    delegateAccount.ProfileImage_deprecated == null ||
                                    delegateAccount.ProfileImage_deprecated.SequenceEqual(existingUser.ProfileImage);

            return firstNamesMatch && lastNamesMatch && profileImageMatch && prnMatch;
        }

        private static void InsertNewUserForDelegateAccount(
            IDbConnection connection,
            DelegateAccount delegateAccount,
            bool setEmailToGuid
        )
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
                                EmailVerified,
                                DetailsLastChecked
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
                                GETUTCDATE(),
                                CASE WHEN @setEmailToGuid = 1 THEN NULL ELSE GETUTCDATE() END
                            )",
                new
                {
                    primaryEmail = setEmailToGuid
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
                    setEmailToGuid
                }
            );

            UpdateDelegateAccountUserIdEmailAndDetailsLastChecked(connection, userId, delegateAccount.Id);
        }

        private static void UpdateDelegateAccountUserIdEmailAndDetailsLastChecked(
            IDbConnection connection,
            int userId,
            int delegateAccountId
        )
        {
            connection.Execute(
                @"UPDATE DelegateAccounts
                            SET
                                UserID = @userId,
                                Email = NULL,
                                CentreSpecificDetailsLastChecked = GETUTCDATE()
                            WHERE ID = @delegateAccountId",
                new
                {
                    userId,
                    delegateAccountId
                }
            );
        }

        private class DelegateAccount
        {
            public int Id { get; set; }
            public bool Active { get; set; }
            public int CentreId { get; set; }
            public string? FirstName_deprecated { get; set; }
            public string LastName_deprecated { get; set; } = null!;
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
            public string PasswordHash { get; set; } = null!;
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
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
