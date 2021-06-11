namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IUserDataService
    {
        public AdminUser? GetAdminUserById(int id);
        public DelegateUser? GetDelegateUserById(int id);
        public List<AdminUser> GetAdminUsersByCentreId(int centreId);
        public AdminUser? GetAdminUserByUsername(string username);
        public List<DelegateUser> GetDelegateUsersByUsername(string username);
        public AdminUser? GetAdminUserByEmailAddress(string emailAddress);
        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress);
        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId);
        public void UpdateAdminUser(string firstName, string surname, string email, byte[]? profileImage, int id);

        public void UpdateDelegateUsers(
            string firstName,
            string surname,
            string email,
            byte[]? profileImage,
            int[] ids
        );

        public void UpdateDelegateUserCentrePrompts(
            int id,
            int jobGroupId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        );

        public void ApproveDelegateUsers(params int[] ids);

        public void RemoveDelegateUser(int delegateId);

        public int GetNumberOfApprovedDelegatesAtCentre(int centreId);

        public int GetNumberOfActiveAdminsAtCentre(int centreId);

        public int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber);

        public void DeleteAllAnswersForPrompt(int centreId, int promptNumber);
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
                        ct.Active AS CentreActive,
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
                        cc.CategoryName,
                        au.Supervisor AS IsSupervisor,
                        au.Trainer AS IsTrainer,
                        au.IsFrameworkDeveloper,
                        au.ProfileImage
                    FROM AdminUsers AS au
                    INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
                    LEFT JOIN CourseCategories AS cc ON cc.CourseCategoryID = au.CategoryID
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
                        cd.DateRegistered,
                        ct.Active AS CentreActive,
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

        public List<AdminUser> GetAdminUsersByCentreId(int centreId)
        {
            var users = connection.Query<AdminUser>(
                @"SELECT
                        AdminID AS Id,
                        CentreID,
                        Email AS EmailAddress,
                        Forename AS FirstName,
                        Surname AS LastName,
                        Password,
                        CentreAdmin AS IsCentreAdmin,
                        IsCentreManager,
                        ContentCreator AS IsContentCreator,
                        ContentManager AS IsContentManager,
                        PublishToAll,
                        SummaryReports,
                        UserAdmin AS IsUserAdmin,
                        CategoryID,
                        Supervisor AS IsSupervisor,
                        Trainer AS IsTrainer,
                        ImportOnly
                    FROM AdminUsers
                    WHERE Active = 1 AND Approved = 1 AND CentreId = @centreId",
                new { centreId }
            ).ToList();

            return users;
        }

        public AdminUser? GetAdminUserByUsername(string username)
        {
            var user = connection.Query<AdminUser>(
                @"SELECT
                        au.AdminID AS Id,
                        au.CentreID,
                        ct.CentreName,
                        ct.Active AS CentreActive,
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
                        cc.CategoryName,
                        au.Supervisor AS IsSupervisor,
                        au.Trainer AS IsTrainer,
                        au.IsFrameworkDeveloper,
                        au.ProfileImage
                    FROM AdminUsers AS au
                    INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
                    LEFT JOIN CourseCategories AS cc ON cc.CourseCategoryID = au.CategoryID
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
                        cd.CentreID,
                        ct.CentreName,
                        ct.Active AS CentreActive,
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

        public AdminUser? GetAdminUserByEmailAddress(string emailAddress)
        {
            return connection.Query<AdminUser>(
                @"SELECT
                        AdminID AS Id,
                        Forename AS FirstName,
                        Surname AS LastName,
                        Email AS EmailAddress,
                        Password,
                        ResetPasswordID
                    FROM AdminUsers
                    WHERE (Email = @emailAddress)",
                new { emailAddress }
            ).SingleOrDefault();
        }

        public List<DelegateUser> GetDelegateUsersByEmailAddress(string emailAddress)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID AS Id,
                        cd.CandidateNumber,
                        cd.CentreID,
                        ct.CentreName,
                        ct.Active AS CentreActive,
                        cd.EmailAddress,
                        cd.FirstName,
                        cd.LastName,
                        cd.Password,
                        cd.Approved,
                        cd.ResetPasswordID
                    FROM Candidates AS cd
                    INNER JOIN Centres AS ct ON ct.CentreID = cd.CentreID
                    WHERE cd.EmailAddress = @emailAddress",
                new { emailAddress }
            ).ToList();

            return users;
        }

        public List<DelegateUser> GetUnapprovedDelegateUsersByCentreId(int centreId)
        {
            List<DelegateUser> users = connection.Query<DelegateUser>(
                @"SELECT
                        cd.CandidateID,
                        cd.CandidateNumber,
                        cd.FirstName,
                        cd.LastName,
                        cd.EmailAddress,
                        cd.DateRegistered,
                        cd.Answer1,
                        cd.Answer2,
                        cd.Answer3,
                        cd.Answer4,
                        cd.Answer5,
                        cd.Answer6,
                        jg.JobGroupName
                    FROM Candidates AS cd
                    INNER JOIN JobGroups AS jg ON jg.JobGroupID = cd.JobGroupID
                    WHERE (cd.Approved = 0)
                    AND (cd.Active = 1)
                    AND (cd.CentreID = @centreId)",
                new { centreId }
            ).ToList();

            return users;
        }

        public void UpdateAdminUser(string firstName, string surname, string email, byte[]? profileImage, int id)
        {
            connection.Execute(
                @"UPDATE AdminUsers
                        SET
                            Forename = @firstName,
                            Surname = @surname,
                            Email = @email,
                            ProfileImage = @profileImage
                        WHERE AdminID = @id",
                new { firstName, surname, email, profileImage, id }
            );
        }

        public void UpdateDelegateUsers(string firstName, string surname, string email, byte[]? profileImage, int[] ids)
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET
                            FirstName = @firstName,
                            LastName = @surname,
                            EmailAddress = @email,
                            ProfileImage = @profileImage
                        WHERE CandidateID in @ids",
                new { firstName, surname, email, profileImage, ids }
            );
        }

        public void UpdateDelegateUserCentrePrompts(
            int id,
            int jobGroupId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET
                            JobGroupId = @jobGroupId,
                            Answer1 = @answer1,
                            Answer2 = @answer2,
                            Answer3 = @answer3,
                            Answer4 = @answer4,
                            Answer5 = @answer5,
                            Answer6 = @answer6
                        WHERE CandidateID = @id",
                new { jobGroupId, answer1, answer2, answer3, answer4, answer5, answer6, id }
            );
        }

        public void ApproveDelegateUsers(params int[] ids)
        {
            connection.Execute(
                @"UPDATE Candidates
                        SET Approved = 1
                        WHERE CandidateID IN @ids",
                new { ids }
            );
        }

        public void RemoveDelegateUser(int delegateId)
        {
            using var transaction = new TransactionScope();
            try
            {
                var existingSessions = connection.Query<int>(
                    @"SELECT SessionID FROM Sessions WHERE CandidateID = @delegateId",
                    new { delegateId }
                );

                if (existingSessions.Any())
                {
                    throw new UserAccountInvalidStateException(
                        $"Delegate user id {delegateId} cannot be removed as they have already started a session."
                        );
                }

                connection.Execute(
                    @"
                    DELETE FROM NotificationUsers
                        WHERE CandidateID = @delegateId

                    DELETE FROM GroupDelegates
                        WHERE DelegateID = @delegateId

                    DELETE FROM Candidates
                        WHERE CandidateID = @delegateId",
                    new { delegateId }
                );
                transaction.Complete();
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
        }

        public int GetNumberOfApprovedDelegatesAtCentre(int centreId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM Candidates WHERE Active = 1 AND Approved = 1 AND CentreID = @centreId",
                new { centreId }
            );
        }

        public int GetNumberOfActiveAdminsAtCentre(int centreId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM AdminUsers WHERE Active = 1 AND CentreID = @centreId",
                new { centreId }
            );
        }

        public int GetDelegateCountWithAnswerForPrompt(int centreId, int promptNumber)
        {
            return connection.Query<string>(
                $@"SELECT Answer{promptNumber}
                        FROM Candidates
                        WHERE CentreID = @centreId AND Answer{promptNumber} IS NOT NULL",
                new { centreId }
            ).Count(x => !string.IsNullOrWhiteSpace(x)); ;
        }

        public void DeleteAllAnswersForPrompt(int centreId, int promptNumber)
        {
            connection.Execute(
                $@"UPDATE Candidates
                        SET Answer{promptNumber} = NULL
                        WHERE CentreID = @centreId",
                new { centreId }
            );
        }
    }
}
