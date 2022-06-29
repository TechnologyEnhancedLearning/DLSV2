namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
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
                CASE WHEN au.CategoryID = 0 THEN NULL ELSE au.CategoryID END AS CategoryId,
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

        private const string BaseSelectAdminAccountQuery =
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
                        CASE
                            WHEN aa.CategoryID IS NULL THEN 'All'
                            ELSE cc.CategoryName
                        END AS CategoryName,
                        aa.IsFrameworkDeveloper,
                        aa.IsFrameworkContributor,
                        aa.IsWorkforceManager,
                        aa.IsWorkforceContributor,
                        aa.IsLocalWorkforceManager,
                        aa.IsNominatedSupervisor,
                        aa.UserID
                    FROM AdminAccounts AS aa
                    LEFT JOIN CourseCategories AS cc ON cc.CourseCategoryID = aa.CategoryID
                    INNER JOIN Centres AS ce ON ce.CentreId = aa.CentreId";

        private const string BaseAdminEntitySelectQuery =
            @"SELECT
                aa.ID,
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
                CASE
                    WHEN aa.CategoryID IS NULL THEN 'All'
                    ELSE cc.CategoryName
                END AS CategoryName,
                aa.IsFrameworkDeveloper,
                aa.IsFrameworkContributor,
                aa.IsWorkforceManager,
                aa.IsWorkforceContributor,
                aa.IsLocalWorkforceManager,
                aa.IsNominatedSupervisor,
                aa.UserID,
                u.ID,
                u.PrimaryEmail,
                u.PasswordHash,
                u.FirstName,
                u.LastName,
                u.JobGroupID,
                jg.JobGroupName,
                u.ProfessionalRegistrationNumber,
                u.ProfileImage,
                u.Active,
                u.ResetPasswordID,
                u.TermsAgreed,
                u.FailedLoginCount,
                u.HasBeenPromptedForPrn,
                u.LearningHubAuthID,
                u.HasDismissedLhLoginWarning,
                u.EmailVerified,
                u.DetailsLastChecked,
                ucd.ID,
                ucd.UserID,
                ucd.CentreID,
                ucd.Email,
                ucd.EmailVerified
            FROM AdminAccounts AS aa
            LEFT JOIN CourseCategories AS cc ON cc.CourseCategoryID = aa.CategoryID
            INNER JOIN Centres AS ce ON ce.CentreId = aa.CentreID
            INNER JOIN Users AS u ON u.ID = aa.UserID
            LEFT JOIN UserCentreDetails AS ucd ON ucd.UserID = u.ID AND ucd.CentreId = aa.CentreID
            INNER JOIN JobGroups AS jg ON jg.JobGroupID = u.JobGroupID";

        public AdminEntity? GetAdminById(int id)
        {
            var sql = $@"{BaseAdminEntitySelectQuery} WHERE aa.ID = @id";

            return connection.Query<AdminAccount, UserAccount, UserCentreDetails, AdminEntity>(
                sql,
                (adminAccount, userAccount, userCentreDetails) => new AdminEntity(
                    adminAccount,
                    userAccount,
                    userCentreDetails
                ),
                new { id },
                splitOn: "ID,ID"
            ).SingleOrDefault();
        }

        [Obsolete("New code should use GetAdminById instead")]
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
            int? categoryId
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
                    adminId,
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

        /// <summary>
        ///     When we reactivate an admin, we must ensure the admin permissions are not
        ///     greater than basic levels. Otherwise, a basic admin would be able to
        ///     "create" admins with more permissions than themselves.
        /// </summary>
        public void ReactivateAdmin(int adminId)
        {
            connection.Execute(
                @"UPDATE AdminAccounts SET
                        Active = 1,
                        IsCentreManager = 0,
                        IsSuperAdmin = 0
                    WHERE ID = @adminId",
                new { adminId }
            );
        }

        public IEnumerable<AdminAccount> GetAdminAccountsByUserId(int userId)
        {
            return connection.Query<AdminAccount>(
                @$"{BaseSelectAdminAccountQuery}
                    WHERE aa.UserID = @userId",
                new { userId }
            );
        }
    }
}
