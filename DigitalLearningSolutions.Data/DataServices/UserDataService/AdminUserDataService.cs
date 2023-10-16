namespace DigitalLearningSolutions.Data.DataServices.UserDataService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
                au.ResetPasswordId,
                au.UserAdmin AS IsSuperAdmin,
                au.SummaryReports AS IsReportsViewer,
                au.IsLocalWorkforceManager,
                au.IsFrameworkDeveloper,
                au.IsWorkforceManager
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
            int? existingId = (int?)connection.ExecuteScalar("SELECT aa.UserID FROM AdminAccounts AS aa INNER JOIN SupervisorDelegates AS sd ON aa.ID = sd.SupervisorAdminID WHERE aa.ID=@adminId", new { adminId });

            if (existingId > 0)
            {
                connection.Execute(
                    @"UPDATE Users SET Active=0 WHERE ID=@existingId",
                    new { existingId }
                );
            }
            else
            {
                connection.Execute(
                    @"DELETE AdminAccounts
                    WHERE ID = @adminId",
                    new { adminId }
                );
            }
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
		                                aa.IsContentManager, aa.IsContentCreator, aa.IsSupervisor, aa.IsTrainer, aa.CategoryID, aa.IsFrameworkDeveloper, aa.IsFrameworkContributor,aa.ImportOnly,
		                                aa.IsWorkforceManager, aa.IsWorkforceContributor, aa.IsLocalWorkforceManager, aa.IsNominatedSupervisor,
		                                u.ID, u.PrimaryEmail, u.FirstName, u.LastName, u.Active, u.FailedLoginCount,
		                                c.CentreID, c.CentreName,
		                                ucd.ID, ucd.Email, ucd.EmailVerified, ucd.CentreID,
                         (SELECT count(*)
                         FROM (
                                SELECT TOP 1 AdminSessions.AdminID FROM AdminSessions WHERE AdminSessions.AdminID = aa.ID
	                            UNION ALL
                                SELECT TOP 1 FrameworkCollaborators.AdminID FROM FrameworkCollaborators WHERE FrameworkCollaborators.AdminID = aa.ID
	                            UNION ALL
                                SELECT TOP 1 SupervisorDelegates.SupervisorAdminID FROM SupervisorDelegates WHERE SupervisorDelegates.SupervisorAdminID = aa.ID
                            ) AS tempTable) AS AdminIdReferenceCount
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
                (adminAccount, userAccount, centre, userCentreDetails, adminIdReferenceCount) => new AdminEntity(
                    adminAccount,
                    userAccount,
                    centre,
                    userCentreDetails,
                    adminIdReferenceCount
                ),
                new { adminId, search, centreId, userStatus, failedLoginThreshold, role, offset, rows },
                splitOn: "ID,ID,CentreID,ID,AdminIdReferenceCount",
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
        public int RessultCount(int adminId, string search, int? centreId, string userStatus, int failedLoginThreshold, string role)
        {
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
            int ResultCount = connection.ExecuteScalar<int>(
                            @$"SELECT  COUNT(*) AS Matches
                            FROM   AdminAccounts AS aa INNER JOIN
                            Users AS u ON aa.UserID = u.ID INNER JOIN
                            Centres AS c ON aa.CentreID = c.CentreID LEFT OUTER JOIN
                            UserCentreDetails AS ucd ON u.ID = ucd.UserID AND c.CentreID = ucd.CentreID {condition}",
                new { adminId, search, centreId, userStatus, failedLoginThreshold, role },
                commandTimeout: 3000
            );
            return ResultCount;
        }
        public async Task<IEnumerable<AdminEntity>> GetAllAdminsExport(
       string search, int offset, int rows, int? adminId, string userStatus, string role, int? centreId, int failedLoginThreshold, int exportQueryRowLimit, int currentRun
       )
        {
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
            }
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
            string BaseSelectQuery = $@"SELECT aa.ID, aa.UserID, aa.CentreID, aa.Active, aa.IsCentreAdmin, aa.IsReportsViewer, aa.IsSuperAdmin, aa.IsCentreManager, 
		                                aa.IsContentManager, aa.IsContentCreator, aa.IsSupervisor, aa.IsTrainer, aa.CategoryID, aa.IsFrameworkDeveloper, aa.IsFrameworkContributor,aa.ImportOnly,
		                                aa.IsWorkforceManager, aa.IsWorkforceContributor, aa.IsLocalWorkforceManager, aa.IsNominatedSupervisor,
		                                u.ID, u.PrimaryEmail, u.FirstName, u.LastName, u.Active, u.FailedLoginCount,
		                                c.CentreID, c.CentreName,
		                                ucd.ID, ucd.Email, ucd.EmailVerified, ucd.CentreID,
                                        1 as AdminIdReferenceCount
                                    FROM   AdminAccounts AS aa WITH (NOLOCK)  INNER JOIN
                                    Users AS u WITH (NOLOCK)  ON aa.UserID = u.ID INNER JOIN
                                    Centres AS c WITH (NOLOCK)  ON aa.CentreID = c.CentreID LEFT OUTER JOIN
                                    UserCentreDetails AS ucd WITH (NOLOCK)  ON u.ID = ucd.UserID AND c.CentreID = ucd.CentreID";
            string sql = @$"{BaseSelectQuery}{condition} ORDER BY LTRIM(u.LastName), LTRIM(u.FirstName)
                            OFFSET @exportQueryRowLimit * (@currentRun - 1) ROWS
                            FETCH NEXT @exportQueryRowLimit ROWS ONLY";
            IEnumerable<AdminEntity> adminEntity = connection.Query<AdminAccount, UserAccount, Centre, UserCentreDetails, int, AdminEntity>(
                sql,
                (adminAccount, userAccount, centre, userCentreDetails, adminIdReferenceCount) => new AdminEntity(
                    adminAccount,
                    userAccount,
                    centre,
                    userCentreDetails,
                    adminIdReferenceCount
                ),
                new { adminId, search, centreId, userStatus, failedLoginThreshold, role, offset, rows, exportQueryRowLimit, currentRun },
                splitOn: "ID,ID,CentreID,ID,AdminIdReferenceCount",
                commandTimeout: 3000
            );
            return adminEntity;
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


        public void UpdateAdminUserAndSpecialPermissions(
            int adminId,
            bool isCentreAdmin,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isContentManager,
            bool importOnly,
            int? categoryId,
            bool isCentreManager,
            bool isSuperAdmin,
            bool isReportsViewer,
            bool isLocalWorkforceManager,
            bool isFrameworkDeveloper,
            bool isWorkforceManager
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
                            IsCentreManager = @isCentreManager,
                            IsSuperAdmin = @isSuperAdmin,
                            IsReportsViewer = @isReportsViewer,
                            IsLocalWorkforceManager = @isLocalWorkforceManager,
                            IsFrameworkDeveloper = @isFrameworkDeveloper,
                            IsWorkforceManager = @isWorkforceManager
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
                    isCentreManager,
                    isSuperAdmin,
                    isReportsViewer,
                    isLocalWorkforceManager,
                    isFrameworkDeveloper,
                    isWorkforceManager
                }
            );
        }

        public int GetUserIdFromAdminId(int adminId)
        {
            return connection.QuerySingle<int>(
                @"SELECT UserID FROM AdminAccounts
                    WHERE ID = @adminId",
                new { adminId }
            );
        }
        public void UpdateAdminCentre(int adminId, int centreId)
        {
            connection.Execute(
            @"UPDATE AdminAccounts
                        SET
                            CentreId = @centreId
                        WHERE ID = @adminId",
            new { adminId, centreId });
        }

        public bool IsUserAlreadyAdminAtCentre(int? userId, int centreId)
        {
            return connection.QueryFirst<int>(
                @$"SELECT COUNT(*)
                     FROM AdminAccounts
                     WHERE CentreId = @centreId AND UserID = @userId",
                new { userId, centreId }
            ) > 0;
        }

        public void LinkAdminAccountToNewUser(
            int currentUserIdForAdminAccount,
            int newUserIdForAdminAccount,
            int centreId
        )
        {
            connection.Execute(
                @"UPDATE AdminAccounts
                    SET UserID = @newUserIdForAdminAccount
                    WHERE UserID = @currentUserIdForAdminAccount AND CentreID = @centreId",
                new { currentUserIdForAdminAccount, newUserIdForAdminAccount, centreId }
            );
        }
    }
}
