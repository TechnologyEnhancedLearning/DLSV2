namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public User? GetUserByEmailAddress(string emailAddress);
        public AdminUser? GetAdminUserByEmailAddress(string emailAddress);
        public DelegateUser? GetDelegateUserByEmailAddress(string emailAddress);

    }

    public class UserService : IUserService
    {
        private readonly IDbConnection connection;

        public UserService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public User? GetUserByEmailAddress(string emailAddress)
        {
            // Assume user is a delegate (in Candidates Table)
            User? user = GetDelegateUserByEmailAddress(emailAddress);

            // If no Candidates entry found, look to AdminUsers
            user ??= GetAdminUserByEmailAddress(emailAddress);

            return user;
        }

        public AdminUser? GetAdminUserByEmailAddress(string emailAddress)
        {
            AdminUser? user = connection.Query<AdminUser>(
                @"SELECT TOP (1)
                        AdminID,
                        Forename,
                        Surname,
                        Email,
                        ResetPasswordID
                    FROM AdminUsers
                    WHERE (Email = @emailAddress)",
                new { emailAddress }
            ).FirstOrDefault();

            return user;
        }

        public DelegateUser? GetDelegateUserByEmailAddress(string emailAddress)
        {
            DelegateUser? user = connection.Query<DelegateUser>(
                @"SELECT TOP (1)
                        CandidateID,
                        FirstName,
                        LastName,
                        EmailAddress,
                        ResetPasswordID
                    FROM Candidates
                    WHERE (EmailAddress = @emailAddress)",
                new { emailAddress }
            ).FirstOrDefault();

            return user;
        }
    }
}
