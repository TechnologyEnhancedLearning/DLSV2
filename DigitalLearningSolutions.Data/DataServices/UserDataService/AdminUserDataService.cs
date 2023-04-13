namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Centres;
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

        public IEnumerable<AdminEntity> GetActiveAdminsByCentreId(int centreId)
        {
            var sql = $@"{BaseAdminEntitySelectQuery} WHERE aa.centreID = @centreId AND aa.Active = 1 AND u.Active = 1";

            return connection.Query<AdminAccount, UserAccount, UserCentreDetails, AdminEntity>(
                sql,
                (adminAccount, userAccount, userCentreDetails) => new AdminEntity(
                    adminAccount,
                    userAccount,
                    userCentreDetails
                ),
                new { centreId }
            );
        }

        [Obsolete("New code should use GetAdminById instead")]
        public AdminUser? GetAdminUserById(int id)
        {
            var user = connection.Query<AdminUser>(
                @$"{BaseSelectAdminQuery}
                    WHERE au.AdminID = @id AND au.Active = 1",
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

        [Obsolete("New code should use GetAdminsByCentreId instead")]
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
            int? categoryId,
            bool isCentreManager
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
                            CategoryID = @categoryId,
                            IsCentreManager = @isCentreManager
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
                    isCentreManager
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

        public (IEnumerable<AdminEntity>, int) GetAllAdmins(
        string search, int offset, int rows, int? adminId, string userStatus, string role, int? centreId, int failedLoginThreshold
        )
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
            }

            string BaseSelectQuery = $@"SELECT aa.ID, aa.UserID, aa.CentreID, aa.Active, aa.IsCentreAdmin, aa.IsReportsViewer, aa.IsSuperAdmin, aa.IsCentreManager, 
		                                aa.IsContentManager, aa.IsContentCreator, aa.IsSupervisor, aa.IsTrainer, aa.CategoryID, aa.IsFrameworkDeveloper, aa.IsFrameworkContributor, 
		                                aa.IsWorkforceManager, aa.IsWorkforceContributor, aa.IsLocalWorkforceManager, aa.IsNominatedSupervisor,
		                                u.ID, u.PrimaryEmail, u.FirstName, u.LastName, u.Active, u.FailedLoginCount,
		                                c.CentreID, c.CentreName,
		                                ucd.ID, ucd.Email, ucd.EmailVerified, ucd.CentreID,
                                        (SELECT COUNT(*) FROM AdminSessions WHERE AdminID = aa.ID) AS AdminSessions
                                    FROM   AdminAccounts AS aa INNER JOIN
                                    Users AS u ON aa.UserID = u.ID INNER JOIN
                                    Centres AS c ON aa.CentreID = c.CentreID LEFT OUTER JOIN
                                    UserCentreDetails AS ucd ON u.ID = ucd.UserID AND c.CentreID = ucd.CentreID";

            string condition = $@" WHERE ((@adminId = 0) OR (aa.ID = @adminId)) AND 
                                (u.FirstName + ' ' + u.LastName + ' ' + u.PrimaryEmail + ' ' + COALESCE(ucd.Email, '') + ' ' + COALESCE(u.ProfessionalRegistrationNumber, '') LIKE N'%' + @search + N'%') AND 
                                ((aa.CentreID = @centreId) OR (@centreId= 0)) AND 
                                ((@userStatus = 'Any') OR (@userStatus = 'Active' AND aa.Active = 1 AND u.Active =1) OR (@userStatus = 'Inactive' AND (u.Active = 0 OR aa.Active =0))) AND
                                ((@role = 'Any') OR 
                                 (@role = 'Super admin' AND aa.IsSuperAdmin = 1) OR (@role = 'Centre manager' AND aa.IsCentreManager = 1) OR 
                                 (@role = 'Centre administrator' AND aa.IsCentreAdmin = 1) OR (@role = 'Supervisor' AND aa.IsSupervisor = 1) OR 
                                 (@role = 'Nominated supervisor' AND aa.IsNominatedSupervisor = 1) OR (@role = 'Trainer' AND aa.IsTrainer = 1) OR 
                                 (@role = 'Content Creator license' AND aa.IsContentCreator = 1) OR (@role = 'CMS administrator' AND aa.IsContentManager = 1 AND aa.ImportOnly =1) OR 
                                 (@role = 'CMS manager' AND aa.IsContentManager = 1 AND aa.ImportOnly = 0))
                                 ";

            string sql = @$"{BaseSelectQuery}{condition} ORDER BY LTRIM(u.LastName), LTRIM(u.FirstName)
                            OFFSET @offset ROWS
                            FETCH NEXT @rows ROWS ONLY";

            IEnumerable<AdminEntity> adminEntity = connection.Query<AdminAccount, UserAccount, Centre, UserCentreDetails, int, AdminEntity>(
                sql,
                (adminAccount, userAccount, centre, userCentreDetails, adminSessions) => new AdminEntity(
                    adminAccount,
                    userAccount,
                    centre,
                    userCentreDetails,
                    adminSessions
                ),
                new { adminId, search, centreId, userStatus, failedLoginThreshold, role, offset, rows },
                splitOn: "ID,ID,CentreID,ID,AdminSessions",
                commandTimeout: 3000
            );

            int ResultCount = connection.ExecuteScalar<int>(
                            @$"SELECT  COUNT(*) AS Matches
                            FROM   AdminAccounts AS aa INNER JOIN
                            Users AS u ON aa.UserID = u.ID INNER JOIN
                            Centres AS c ON aa.CentreID = c.CentreID LEFT OUTER JOIN
                            UserCentreDetails AS ucd ON u.ID = ucd.UserID AND c.CentreID = ucd.CentreID {condition}",
                new { adminId, search, centreId, userStatus, failedLoginThreshold, role },
                commandTimeout: 3000
            );
            return (adminEntity, ResultCount);
        }

        public void UpdateAdminStatus(int adminId, bool active)
        {
            connection.Execute(
                @"UPDATE AdminAccounts SET
                        Active = @active
                    WHERE ID = @adminId",
                new { active, adminId }
            );
        }
    }


}
