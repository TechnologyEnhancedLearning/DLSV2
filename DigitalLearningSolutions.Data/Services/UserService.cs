namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username);
        public AdminUser? GetAdminUserByUsername(string username);
        public List<DelegateUser> GetDelegateUsersByUsername(string username);
        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress);
    }

    public class UserService : IUserService
    {
        private readonly IDbConnection connection;

        public UserService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public (AdminUser?, List<DelegateUser>) GetUsersByUsername(string username)
        {
            var adminUser = GetAdminUserByUsername(username);
            var delegateUsername =
                string.IsNullOrWhiteSpace(adminUser?.EmailAddress) ? username : adminUser.EmailAddress;
            List<DelegateUser> delegateUsers = GetDelegateUsersByUsername(delegateUsername);

            return (adminUser, delegateUsers);
        }

        public AdminUser? GetAdminUserByUsername(string username)
        {
            var user = connection.Query<AdminUser>(
                @"SELECT
                        au.AdminID AS Id,
                        au.CentreID, 
                        ct.CentreName,
                        au.Email AS EmailAddress,
                        au.Forename AS FirstName,
                        au.Surname AS LastName,
                        au.Password,
                        au.CentreAdmin AS IsCentreAdmin,
                        au.IsCentreManager,
                        au.ContentCreator AS IsContentCreator,
                        au.ContentManager AS IsContentManager,
                        au.PublishToAll,
                        au.SummaryReports,
                        au.UserAdmin AS IsUserAdmin,
                        au.CategoryID,
                        au.Supervisor AS IsSupervisor,
                        au.Trainer AS IsTrainer,
                        au.IsFrameworkDeveloper,
                        au.Login
                    FROM AdminUsers AS au
                    INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
                    WHERE au.Active = 1 AND au.Approved = 1 AND (au.Login = @username OR au.Email = @username)",
                new { username }
            ).FirstOrDefault();

            return user;
        }

        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress)
        {
            List<AdminUser> adminUsers = GetAdminUsersByEmailAddress(emailAddress);
            List<DelegateUser> delegateUsers = GetDelegateUsersByEmailAddress(emailAddress);

            return (adminUsers, delegateUsers);
        }

        public List<DelegateUser> GetDelegateUsersByUsername(string username)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID AS Id,
                        cd.CandidateNumber,
                        ct.CentreName,
                        cd.CentreID,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID 
                    WHERE cd.Active = 1 AND
                         (cd.CandidateNumber = @username OR cd.EmailAddress = @username OR cd.AliasID = @username)",
                new { username }
            ).ToList();

            return users;
        }

        private List<AdminUser> GetAdminUsersByEmailAddress(string emailAddress)
        {
            List<AdminUser> users = connection.Query<AdminUser>(
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

            return users;
        }

        private List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
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

            return users;
        }
    }
}
