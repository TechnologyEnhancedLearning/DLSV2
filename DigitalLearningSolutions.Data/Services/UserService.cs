namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserService
    {
        public List<AdminUser> GetAdminUsersByUsername(string username);
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

        public List<AdminUser> GetAdminUsersByUsername(string username)
        {
            List<AdminUser> users = connection.Query<AdminUser>(
                @"SELECT
                        au.AdminID,
                        au.CentreID, 
                        ct.CentreName,
                        au.Email,
                        au.Forename,
                        au.Surname,
                        au.Password,
                        au.CentreAdmin,
                        au.IsCentreManager,
                        au.ContentCreator,
                        au.ContentManager,
                        au.PublishToAll,
                        au.SummaryReports,
                        au.UserAdmin,
                        au.CategoryID,
                        au.Supervisor,
                        au.Trainer,
                        au.IsFrameworkDeveloper
                    FROM AdminUsers AS au
                    INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
                    WHERE au.Active = 1 AND au.Approved = 1 AND (au.Login = @username OR au.Email = @username)",
                new { username }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetDelegateUsersByUsername(string username)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID,
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

        public (List<AdminUser>, List<DelegateUser>) GetUsersByEmailAddress(string emailAddress)
        {
            List<AdminUser> adminUsers = GetAdminUsersByEmailAddress(emailAddress);
            List<DelegateUser> delegateUsers = GetDelegateUsersByEmailAddress(emailAddress);

            return (adminUsers, delegateUsers);
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
