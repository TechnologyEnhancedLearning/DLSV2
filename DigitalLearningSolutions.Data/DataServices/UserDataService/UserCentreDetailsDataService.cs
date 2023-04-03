namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        public void SetCentreEmail(
            int userId,
            int centreId,
            string? email,
            DateTime? emailVerified,
            IDbTransaction? transaction = null
        )
        {
            var transactionShouldBeClosed = false;
            if (transaction == null)
            {
                connection.EnsureOpen();
                transaction = connection.BeginTransaction();
                transactionShouldBeClosed = true;
            }

            var userCentreDetailsValues = new
            {
                userId,
                centreId,
                email,
                emailVerified,
            };

            var detailsAlreadyExist = connection.QuerySingle<bool>(
                @"SELECT CASE
                        WHEN EXISTS (SELECT ID FROM UserCentreDetails
                            WHERE UserID = @userId AND CentreID = @centreId)
                        THEN 1
                        ELSE 0
                        END",
                new { userId, centreId },
                transaction
            );

            if (detailsAlreadyExist)
            {
                connection.Execute(
                    string.IsNullOrWhiteSpace(email)
                        ? @"UPDATE UserCentreDetails
                        SET Email = NULL, EmailVerified = NULL
                        WHERE userID = @userId AND centreID = @centreId"
                        : @"UPDATE UserCentreDetails
                        SET Email = @email, EmailVerified = @emailVerified
                        WHERE userID = @userId AND centreID = @centreId",
                    userCentreDetailsValues,
                    transaction
                );
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails
                    (
                        UserId,
                        CentreId,
                        Email,
                        EmailVerified
                    )
                    VALUES
                    (
                        @userId,
                        @centreId,
                        @email,
                        @emailVerified
                    )",
                    userCentreDetailsValues,
                    transaction
                );
            }

            if (transactionShouldBeClosed)
            {
                transaction.Commit();
            }
        }

        public bool CentreSpecificEmailIsInUseAtCentre(string email, int centreId)
        {
            return CentreSpecificEmailIsInUseAtCentreQuery(email, centreId, null);
        }

        public bool CentreSpecificEmailIsInUseAtCentreByOtherUser(
            string email,
            int centreId,
            int userId
        )
        {
            return CentreSpecificEmailIsInUseAtCentreQuery(email, centreId, userId);
        }

        public string? GetCentreEmail(int userId, int centreId)
        {
            return connection.QuerySingleOrDefault<string>(
                @"SELECT Email
                    FROM UserCentreDetails
                    WHERE UserID = @userId AND CentreID = @centreId",
                new { userId, centreId }
            );
        }

        public IEnumerable<EmailVerificationDetails> GetCentreEmailVerificationDetails(string code)
        {
            return connection.Query<EmailVerificationDetails>(
                @"SELECT
                        u.UserId,
                        u.Email,
                        u.EmailVerified,
                        h.EmailVerificationHash,
                        h.CreatedDate AS EmailVerificationHashCreatedDate,
                        IIF(da.Approved = 0, da.CentreID, NULL) AS CentreIdIfEmailIsForUnapprovedDelegate
                    FROM UserCentreDetails u
                    JOIN EmailVerificationHashes h ON h.ID = u.EmailVerificationHashID
                    JOIN UserCentreDetails ucd ON ucd.Email = u.Email
                    JOIN DelegateAccounts da ON da.UserID = ucd.UserID AND da.CentreID = ucd.CentreID
                    WHERE h.EmailVerificationHash = @code",
                new { code }
            );
        }

        public void SetCentreEmailVerified(int userId, string email, DateTime verifiedDateTime)
        {
            connection.Execute(
                @"UPDATE UserCentreDetails
                    SET EmailVerified = @verifiedDateTime, EmailVerificationHashID = NULL
                    WHERE UserID = @userId AND Email = @email AND EmailVerified IS NULL",
                new { userId, email, verifiedDateTime }
            );
        }

        public IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> GetAllActiveCentreEmailsForUser(
            int userId, bool isAll = false
        )
        {
            string delegateAccount = "SELECT c.CentreId, c.CentreName, ucd.Email FROM DelegateAccounts AS da INNER JOIN Centres AS c ON c.CentreID = da.CentreID LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID AND ucd.CentreID = c.CentreID WHERE da.UserID = ";
            string adminAccount = " SELECT c.CentreId, c.CentreName, ucd.Email FROM AdminAccounts AS aa INNER JOIN Centres AS c ON c.centreID = aa.CentreID LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = aa.UserID AND ucd.CentreID = c.CentreID WHERE aa.UserID = ";

            if (!isAll)
            {
                delegateAccount += userId + " AND da.Active = 1";
                adminAccount += userId + " AND aa.Active = 1 ";
            }
            else
            {
                delegateAccount += userId;
                adminAccount += userId;
            }
            return connection.Query<(int, string, string?)>(
                $"{delegateAccount} UNION {adminAccount}"
            );
        }

        public IEnumerable<(int centreId, string centreName, string? centreSpecificEmail)> GetAllCentreEmailsForUser(
            int userId
        )
        {
            return connection.Query<(int, string, string?)>(
                @"SELECT c.CentreId, c.CentreName, ucd.Email
                    FROM DelegateAccounts AS da
                    INNER JOIN Centres AS c ON c.CentreID = da.CentreID
                    LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = da.UserID AND ucd.CentreID = c.CentreID
                    WHERE da.UserID = @userId

                    UNION

                    SELECT c.CentreId, c.CentreName, ucd.Email
                    FROM AdminAccounts AS aa
                    INNER JOIN Centres AS c ON c.centreID = aa.CentreID
                    LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = aa.UserID AND ucd.CentreID = c.CentreID
                    WHERE aa.UserID = @userId",
                new { userId }
            );
        }

        public IEnumerable<(int centreId, string centreName, string centreEmail)> GetUnverifiedCentreEmailsForUser(
            int userId
        )
        {
            return connection.Query<(int, string, string)>(
                @"SELECT
                        c.CentreID,
                        c.CentreName,
                        ucd.Email
                    FROM UserCentreDetails AS ucd
                    INNER JOIN Centres AS c ON c.CentreID = ucd.CentreID
                    WHERE ucd.UserID = @userId
                        AND ucd.Email IS NOT NULL
                        AND ucd.EmailVerified IS NULL
                        AND c.Active = 1",
                new { userId }
            );
        }

        public (int? userId, int? centreId, string? centreName)
            GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(
                string centreSpecificEmail,
                string registrationConfirmationHash
            )
        {
            return connection.QuerySingleOrDefault<(int?, int?, string?)>(
                @"SELECT ucd.UserID, c.CentreID, c.CentreName
                    FROM UserCentreDetails AS ucd
                    INNER JOIN DelegateAccounts AS da ON da.UserID = ucd.UserID AND da.CentreID = ucd.CentreID
                    INNER JOIN Centres AS c ON c.CentreID = ucd.CentreID
                    WHERE ucd.Email = @centreSpecificEmail
                        AND da.RegistrationConfirmationHash = @registrationConfirmationHash",
                new { centreSpecificEmail, registrationConfirmationHash }
            );
        }

        public void LinkUserCentreDetailsToNewUser(
            int currentUserIdForUserCentreDetails,
            int newUserIdForUserCentreDetails,
            int centreId
        )
        {
            connection.Execute(
                @"UPDATE UserCentreDetails
                    SET UserID = @newUserIdForUserCentreDetails
                    WHERE UserID = @currentUserIdForUserCentreDetails AND CentreID = @centreId",
                new { currentUserIdForUserCentreDetails, newUserIdForUserCentreDetails, centreId }
            );
        }

        public IEnumerable<UserCentreDetails> GetCentreDetailsForUser(int userId)
        {
            return connection.Query<UserCentreDetails>(
                @"SELECT ID, UserID, CentreID, Email, EmailVerified FROM UserCentreDetails WHERE UserID = @userId",
                new { userId }
            );
        }

        private bool CentreSpecificEmailIsInUseAtCentreQuery(
            string email,
            int centreId,
            int? userId
        )
        {
            return connection.QueryFirst<int>(
                @$"SELECT COUNT(*)
                    FROM UserCentreDetails
                    WHERE CentreId = @centreId AND Email = @email
                    {(userId == null ? "" : "AND UserId <> @userId")}",
                new { email, centreId, userId }
            ) > 0;
        }

        public void DeleteUserCentreDetail(
            int userId,
            int centreId
        )
        {
            connection.Execute(
                @"DELETE FROM UserCentreDetails
                    WHERE UserID = @userId AND CentreID = @centreId",
                new { userId, centreId }
            );
        }

        public bool PrimaryEmailIsInUseAtCentre(string email, int centreId)
        {
            return connection.QueryFirst<int>(
                @$"SELECT COUNT(*)
                                FROM Users AS u
                                INNER JOIN UserCentreDetails AS ucd ON u.ID = ucd.UserID
                                WHERE ucd.CentreId = @centreId AND u.PrimaryEmail = @email",
                new { email, centreId }
            ) > 0;
        }
    }
}
