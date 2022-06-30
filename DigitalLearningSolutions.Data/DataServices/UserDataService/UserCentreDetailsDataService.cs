namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Extensions;

    public partial class UserDataService
    {
        public void SetCentreEmail(
            int userId,
            int centreId,
            string? email,
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
                        SET Email = NULL,
                            EmailVerified = NULL
                        WHERE userID = @userId AND centreID = @centreId"
                        : @"UPDATE UserCentreDetails
                        SET Email = @email
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
                        Email
                    )
                    VALUES
                    (
                        @userId,
                        @centreId,
                        @email
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

        public bool CentreSpecificEmailIsInUseAtCentre(string email, int centreId, IDbTransaction? transaction = null)
        {
            return connection.QueryFirst<int>(
                @"SELECT COUNT(*)
                    FROM UserCentreDetails
                    WHERE CentreId = @centreId AND Email = @email",
                new { email, centreId },
                transaction
            ) > 0;
        }

        public string? GetCentreEmail(int userId, int centreId)
        {
            return connection.Query<string?>(
                @"SELECT Email
                    FROM UserCentreDetails
                    WHERE UserID = @userId AND CentreID = @centreId",
                new { userId, centreId }
            ).SingleOrDefault();
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
    }
}
