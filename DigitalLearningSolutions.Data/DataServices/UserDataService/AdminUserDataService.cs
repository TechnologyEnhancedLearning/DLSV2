namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.User;

    public partial class UserDataService
    {
        private const string BaseSelectAdminQuery =
            @"SELECT
                au.AdminID AS Id,
                au.CentreID,
                ct.CentreName,
                ct.Active AS CentreActive,
                au.Active,
                au.Approved,
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
                CASE
                    WHEN au.CategoryID = 0 THEN 'All'
                    ELSE cc.CategoryName
                END AS CategoryName,
                au.Supervisor AS IsSupervisor,
                au.NominatedSupervisor AS IsNominatedSupervisor,
                au.Trainer AS IsTrainer,
                au.IsFrameworkDeveloper,
                au.ProfileImage,
                au.IsFrameworkContributor,
                au.IsWorkforceManager,
                au.IsWorkforceContributor,
                au.IsLocalWorkforceManager,
                au.ImportOnly,
                au.FailedLoginCount,
                au.ResetPasswordId
            FROM AdminUsers AS au
            INNER JOIN Centres AS ct ON ct.CentreID = au.CentreID
            LEFT JOIN CourseCategories AS cc ON cc.CourseCategoryID = au.CategoryID";

        public AdminUser? GetAdminUserById(int id)
        {
            var user = connection.Query<AdminUser>(
                @$"{BaseSelectAdminQuery}
                    WHERE au.AdminID = @id",
                new { id }
            ).SingleOrDefault();

            return user;
        }

        public AdminUser? GetAdminUserByEmailAddress(string emailAddress)
        {
            return connection.Query<AdminUser>(
                @$"{BaseSelectAdminQuery}
                    WHERE (au.Email = @emailAddress)",
                new { emailAddress }
            ).SingleOrDefault();
        }

        public List<AdminUser> GetAdminUsersByCentreId(int centreId)
        {
            var users = connection.Query<AdminUser>(
                @$"{BaseSelectAdminQuery}
                    WHERE au.Active = 1 AND au.Approved = 1 AND au.CentreId = @centreId",
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

        public int GetNumberOfActiveAdminsAtCentre(int centreId)
        {
            return (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM AdminUsers WHERE Active = 1 AND CentreID = @centreId",
                new { centreId }
            );
        }

        public void UpdateAdminUserPermissions(
            int adminId,
            bool isCentreAdmin,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isContentManager,
            bool importOnly,
            int categoryId
        )
        {
            connection.Execute(
                @"UPDATE AdminAccounts
                        SET
                            IsCentreAdmin = @isCentreAdmin,
                            IsSupervisor = @isSupervisor,
                            IsNominatedSupervisor = @isNominatedSupervisor,
                            IsTrainer = @isTrainer,
                            IsContentCreator = @isContentCreator,
                            IsContentManager = @isContentManager,
                            ImportOnly = @importOnly,
                            CategoryID = @categoryId
                        WHERE ID = @adminId",
                new
                {
                    isCentreAdmin,
                    isSupervisor,
                    isNominatedSupervisor,
                    isTrainer,
                    isContentCreator,
                    isContentManager,
                    importOnly,
                    categoryId,
                    adminId
                }
            );
        }

        public void DeactivateAdmin(int adminId)
        {
            connection.Execute(
                @"UPDATE AdminUsers
                        SET
                            Active = 0
                        WHERE AdminID = @adminId",
                new { adminId }
            );
        }

        public void DeleteAdminAccount(int adminId)
        {
            connection.Execute(
                @"DELETE AdminAccounts
                    WHERE ID = @adminId",
                new { adminId }
            );
        }

        // TODO: do we want to set the category name to "All" when the category id is null?
        public IEnumerable<AdminAccount> GetAdminAccountsByUserId(int userId)
        {
            return connection.Query<AdminAccount>(
                @"SELECT aa.ID,
                        aa.CentreID,
                        ce.CentreName,
                        ce.Active AS CentreActive,
                        aa.IsCentreAdmin,
                        aa.IsReportsViewer,
                        aa.IsSuperAdmin,
                        aa.IsCentreManager,
                        aa.Active,
                        aa.IsContentManager,
                        aa.PublishToAll,
                        aa.ImportOnly,
                        aa.IsContentCreator,
                        aa.IsSupervisor,
                        aa.IsTrainer,
                        aa.CategoryID,
                        cc.CategoryName,
                        aa.IsFrameworkDeveloper,
                        aa.IsFrameworkContributor,
                        aa.IsWorkforceManager,
                        aa.IsWorkforceContributor,
                        aa.IsLocalWorkforceManager,
                        aa.IsNominatedSupervisor,
                        aa.UserID
                    FROM AdminAccounts AS aa
                    LEFT JOIN CourseCategories AS cc ON cc.CourseCategoryID = aa.CategoryID
                    INNER JOIN Centres AS ce ON ce.CentreId = aa.CentreId
                    WHERE aa.UserID = @userId",
                new { userId }
            );
        }
    }
}
