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

        public AdminUser? GetAdminUserByUsername(string username)
        {
            return connection.Query<AdminUser>(
                @$"{BaseSelectAdminQuery}
                    WHERE au.Active = 1 AND au.Approved = 1 AND (au.Login = @username OR au.Email = @username)",
                new { username }
            ).SingleOrDefault();
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
            int categoryId,
            bool isCentreManager
        )
        {
            connection.Execute(
                @"UPDATE AdminUsers
                        SET
                            CentreAdmin = @isCentreAdmin,
                            Supervisor = @isSupervisor,
                            NominatedSupervisor = @isNominatedSupervisor,
                            Trainer = @isTrainer,
                            ContentCreator = @isContentCreator,
                            ContentManager = @isContentManager,
                            ImportOnly = @importOnly,
                            CategoryID = @categoryId,
                            IsCentreManager = @isCentreManager
                        WHERE AdminID = @adminId",
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
                    adminId,
                    isCentreManager
                }
            );
        }

        public void UpdateAdminUserFailedLoginCount(int adminId, int updatedCount)
        {
            connection.Execute(
                @"UPDATE AdminUsers
                        SET
                            FailedLoginCount = @updatedCount
                        WHERE AdminID = @adminId",
                new { adminId, updatedCount }
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

        /// <summary>
        /// When we reactivate an admin, we must ensure the admin permissions are not
        /// greater than basic levels. Otherwise, a basic admin would be able to
        /// "create" admins with more permissions than themselves.
        /// </summary>
        public void ReactivateAdmin(int adminId)
        {
            connection.Execute(
                @"UPDATE AdminUsers SET
                        Active = 1,
                        IsCentreManager = 0,
                        UserAdmin = 0
                    WHERE AdminID = @adminId",
                new { adminId }
            );
        }

        public void DeleteAdminUser(int adminId)
        {
            connection.Execute(
                @"DELETE AdminUsers
                    WHERE AdminID = @adminId",
                new { adminId }
            );
        }
    }
}
