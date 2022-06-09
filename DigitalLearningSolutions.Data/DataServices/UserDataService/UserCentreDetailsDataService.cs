namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
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

            if (EmailIsInUseByOtherUser(userId, email, transaction))
            {
                // It's possible that another user could take this email
                // in the milliseconds between when we do this check and when we save this user's email.
                // We decided our user base (~500,000 in May 2022) was small enough that this isn't likely to happen.
                // This also goes for updating a user's primary email.
                //
                // If this proves to be a problem, we can protect ourselves by doing this:
                // - Also check that no other user is using the email after we update UserCentreDetails.
                // - Wrap the update query, along with both checks, in a transaction.
                // - Roll the transaction back if the second check fails.
                throw new DelegateCreationFailedException(DelegateCreationError.EmailAlreadyInUse);
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
                    @"UPDATE UserCentreDetails
                    SET Email = @email
                    WHERE userID = @userId AND centreID = @centreId",
                    userCentreDetailsValues,
                    transaction
                );
            }
            else
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

        public bool EmailIsInUseByOtherUser(int userId, string email, IDbTransaction? transaction = null)
        {
            return connection.QuerySingle<bool>(
                @"SELECT CASE
                        WHEN EXISTS (SELECT ID FROM USERS WHERE PrimaryEmail = @email AND ID <> @userId)
                        OR EXISTS (SELECT ID FROM UserCentreDetails d
                            WHERE d.Email = @email AND d.UserID <> @userId)
                        THEN 1
                        ELSE 0
                        END",
                new { email, userId },
                transaction
            );
        }

        public bool AnyEmailsInSetAreAlreadyInUse(IEnumerable<string> emails, IDbTransaction? transaction = null)
        {
            return connection.QueryFirst<bool>(
                @"SELECT CASE
                        WHEN EXISTS (SELECT ID FROM Users WHERE PrimaryEmail IN @emails)
                            OR EXISTS (SELECT ID FROM UserCentreDetails d WHERE d.Email IN @emails)
                        THEN 1
                        ELSE 0
                        END",
                new { emails },
                transaction
            );
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
    }
}
