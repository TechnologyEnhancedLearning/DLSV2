namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;

    public partial class UserDataService
    {
        public void SetCentreEmail(
            int userId,
            int centreId,
            string email,
            IDbTransaction? transaction = null
        )
        {
            if (transaction == null)
            {
                connection.EnsureOpen();
                transaction = connection.BeginTransaction();
            }

            if (EmailIsInUseByOtherUser(userId, email, transaction))
            {
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
                    transaction);
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
                transaction);
        }

        public bool AnyEmailsInSetAreAlreadyInUse(IEnumerable<string?> emails, IDbTransaction? transaction = null)
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
    }
}
