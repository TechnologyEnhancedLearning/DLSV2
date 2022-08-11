namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface IEmailVerificationDataService
    {
        int CreateEmailVerificationHash(string hash, DateTime created);

        void UpdateEmailVerificationHashIdForPrimaryEmail(int userId, int hashId);

        void UpdateEmailVerificationHashIdForCentreEmail(int userId, int centreId, int hashId);

        bool AccountEmailIsVerifiedForUser(int userId, string? email);
    }

    public class EmailVerificationDataService : IEmailVerificationDataService
    {
        private readonly IDbConnection connection;

        public EmailVerificationDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int CreateEmailVerificationHash(string hash, DateTime created)
        {
            return connection.QuerySingle<int>(
                @"INSERT INTO EmailVerificationHashes (EmailVerificationHash, CreatedDate)
                        OUTPUT INSERTED.ID
                        VALUES (@hash, @created)",
                new { hash, created }
            );
        }

        public void UpdateEmailVerificationHashIdForPrimaryEmail(int userId, int hashId)
        {
            connection.Execute(
                @"UPDATE Users SET EmailVerificationHashID = @hashId WHERE ID = @userId",
                new { hashId, userId }
            );
        }

        public void UpdateEmailVerificationHashIdForCentreEmail(int userId, int centreId, int hashId)
        {
            connection.Execute(
                @"UPDATE UserCentreDetails
                        SET EmailVerificationHashID = @hashId
                        WHERE UserID = @userId AND CentreID = @centreID",
                new { hashId, userId, centreId }
            );
        }

        public bool AccountEmailIsVerifiedForUser(int userId, string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var isEmailVerifiedAsPrimaryEmail = connection.Query<DateTime?>(
                @"SELECT EmailVerified FROM Users WHERE ID = @userId AND PrimaryEmail = @email",
                new { userId, email }
            ).SingleOrDefault() != null;

            var isEmailVerifiedAsCentreEmail = connection.Query<DateTime?>(
                @"SELECT EmailVerified FROM UserCentreDetails WHERE UserID = @userId AND Email = @email",
                new { userId, email }
            ).Any(date => date != null);

            return isEmailVerifiedAsPrimaryEmail || isEmailVerifiedAsCentreEmail;
        }
    }
}
