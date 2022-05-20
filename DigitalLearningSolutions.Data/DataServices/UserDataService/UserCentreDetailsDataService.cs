namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;

    public partial class UserDataService
    {
        public void CreateOrUpdateUserCentreDetails(
            int userId,
            int centreId,
            string email,
            IDbTransaction? transaction = null
        )
        {
            if (EmailIsInUseByOtherUser(userId, email))
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

        public bool EmailIsInUseByOtherUser(int userId, string email)
        {
            return connection.QuerySingle<bool>(
                @"SELECT CASE
                        WHEN EXISTS (SELECT ID FROM USERS WHERE PrimaryEmail = @email AND ID <> @userId)
                        OR EXISTS (SELECT ID FROM UserCentreDetails d
                            WHERE d.Email = @email AND d.Email IS NOT NULL AND d.UserID <> @userId)
                        THEN 1
                        ELSE 0
                        END",
                new { email, userId });
        }

        public bool AnyEmailsInSetAreAlreadyInUse(IEnumerable<string?> emails)
        {
            return connection.QueryFirst<bool>(
                @"SELECT CASE
                        WHEN EXISTS (SELECT ID FROM Users WHERE PrimaryEmail IN @emails)
                            OR EXISTS (SELECT ID FROM UserCentreDetails d WHERE d.Email IN @emails AND d.Email IS NOT NULL)
                        THEN 1
                        ELSE 0
                        END",
                new { emails }
            );
        }
    }
}
