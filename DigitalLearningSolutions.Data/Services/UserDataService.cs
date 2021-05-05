namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserDataService
    {
        public AdminUser? GetAdminUserById(int id);
        public DelegateUser? GetDelegateUserById(int id);
        public AdminUser? GetAdminUserByUsername(string username);
        public List<DelegateUser> GetDelegateUsersByUsername(string username);
        public List<AdminUser> GetAdminUsersByEmailAddress(string emailAddress);
        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);
        public void UpdateAdminUser(string firstName, string surname, string email, int id);
        public void UpdateDelegateUser(string firstName, string surname, string email, int id);
    }

    public class UserDataService : IUserDataService
    {
        private readonly IDbConnection connection;

        public UserDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public AdminUser? GetAdminUserById(int id)
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
                        au.ProfileImage
                    FROM AdminUsers AS au
                    INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
                    WHERE au.AdminID = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public DelegateUser? GetDelegateUserById(int id)
        {
            var user = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID AS Id,
                        cd.CandidateNumber,
                        ct.CentreName,
                        cd.CentreID,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved,
                        cd.ProfileImage,
                        cd.Answer1,
                        cd.Answer2,
                        cd.Answer3,
                        cd.Answer4,
                        cd.Answer5,
                        cd.Answer6,
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE cd.CandidateId = @id",
                new { id }
            ).SingleOrDefault();

            return user;
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
                        au.ProfileImage
                    FROM AdminUsers AS au
                    INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
                    WHERE au.Active = 1 AND au.Approved = 1 AND (au.Login = @username OR au.Email = @username)",
                new { username }
            ).FirstOrDefault();

            return user;
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

        public List<AdminUser> GetAdminUsersByEmailAddress(string emailAddress)
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

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
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

        public void UpdateAdminUser(string firstName, string surname, string email, int id)
        {
            connection.Execute(
                @"UPDATE AdminUsers
                        SET
                            Forename = @firstName,
                            Surname = @surname,
                            Email = @email
                        WHERE AdminID = @id",
                new {firstName, surname, email, id}
            );
        }

        public void UpdateDelegateUser(string firstName, string surname, string email, int id)
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            EmailAddress = @email
                        WHERE CandidateID = @id",
                new { firstName, surname, email, id }
            );
        }
    }
}
