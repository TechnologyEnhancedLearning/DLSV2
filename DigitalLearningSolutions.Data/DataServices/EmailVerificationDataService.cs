namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface IEmailVerificationDataService
    {
        int CreateEmailVerificationHash(string hash, DateTime created);

        void UpdateEmailVerificationHashIdForPrimaryEmail(int userId, string? emailAddress, int hashId);

        void UpdateEmailVerificationHashIdForCentreEmails(int userId, string? emailAddress, int hashId);

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

        public void UpdateEmailVerificationHashIdForPrimaryEmail(int userId, string? emailAddress, int hashId)
        {
            connection.Execute(
                @"UPDATE Users SET EmailVerificationHashID = @hashId WHERE ID = @userId AND PrimaryEmail = @emailAddress",
                new { hashId, userId, emailAddress }
            );
        }

        public void UpdateEmailVerificationHashIdForCentreEmails(int userId, string? emailAddress, int hashId)
        {
            connection.Execute(
                @"UPDATE UserCentreDetails SET EmailVerificationHashID = @hashId WHERE UserID = @userId AND Email = @emailAddress",
                new { hashId, userId, emailAddress }
            );
        }

        public bool AccountEmailIsVerifiedForUser(int userId, string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var isEmailVerifiedAsPrimaryEmail = connection.QuerySingleOrDefault<DateTime?>(
                @"SELECT EmailVerified FROM Users WHERE ID = @userId AND PrimaryEmail = @email",
                new { userId, email }
            ) != null;

            var isEmailVerifiedAsCentreEmail = connection.Query<DateTime?>(
                @"SELECT EmailVerified FROM UserCentreDetails WHERE UserID = @userId AND Email = @email",
                new { userId, email }
            ).Any(date => date != null);

            return isEmailVerifiedAsPrimaryEmail || isEmailVerifiedAsCentreEmail;
        }
    }
}
