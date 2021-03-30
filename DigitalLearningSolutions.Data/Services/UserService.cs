namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress);
    }

    public class UserService : IUserService
    {
        private readonly IDbConnection connection;

        public UserService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress)
        {
            List<AdminUser> adminUsers = GetAdminUsersByEmailAddress(emailAddress);
            List<DelegateUser> delegateUsers = GetDelegateUsersByEmailAddress(emailAddress);

            return (adminUsers, delegateUsers);
        }

        private List<AdminUser> GetAdminUsersByEmailAddress(string emailAddress)
        {
            List<AdminUser> user = connection.Query<AdminUser>(
                @"SELECT
                        AdminID,
                        Forename,
                        Surname,
                        Email,
                        ResetPasswordID
                    FROM AdminUsers
                    WHERE (Email = @emailAddress)",
                new { emailAddress }
            ).ToList();

            return user;
        }

        private List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
        {
            List<DelegateUser> user = connection.Query<DelegateUser>(
                @"SELECT
                        CandidateID,
                        FirstName,
                        LastName,
                        EmailAddress,
                        ResetPasswordID
                    FROM Candidates
                    WHERE (EmailAddress = @emailAddress)",
                new { emailAddress }
            ).ToList();

            return user;
        }
    }
}
