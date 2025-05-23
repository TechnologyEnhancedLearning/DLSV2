﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface ISupervisorDataService
    {
        //GET DATA
        DashboardData? GetDashboardDataForAdminId(int adminId);
        IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId, int? adminIdCategoryID);
        SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId, int adminId, int delegateUserId);
        SupervisorDelegate GetSupervisorDelegate(int adminId, int delegateUserId);
        int? ValidateDelegate(int centreId, string delegateEmail);
        IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int? adminIdCategoryId);
        DelegateSelfAssessment? GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId, int? adminIdCategoryId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedSignOffs(int adminId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedReviews(int adminId);
        DelegateSelfAssessment? GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId, int? adminIdCategoryId);
        IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int candidateId, int centreId, int? categoryId);
        RoleProfile? GetRoleProfileById(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesForSelfAssessment(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesBySelfAssessmentIdForSupervisor(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetDelegateNominatableSupervisorRolesForSelfAssessment(int selfAssessmentId);
        SelfAssessmentSupervisorRole? GetSupervisorRoleById(int id);
        DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(int selfAssessmentId, int supervisorDelegateId);
        DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(int candidateAssessmentId, int supervisorDelegateId);
        CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisorById(int candidateAssessmentSupervisorId);
        CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisor(int candidateAssessmentID, int supervisorDelegateId, int selfAssessmentSupervisorRoleId);
        SelfAssessmentResultSummary? GetSelfAssessmentResultSummary(int candidateAssessmentId, int supervisorDelegateId);
        IEnumerable<CandidateAssessmentSupervisorVerificationSummary> GetCandidateAssessmentSupervisorVerificationSummaries(int candidateAssessmentId);
        IEnumerable<SupervisorForEnrolDelegate> GetSupervisorForEnrolDelegate(int CentreID, int CategoryID);
        IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminIdWithoutRemovedClause(int adminId);
        SupervisorDelegateDetail GetSupervisorDelegateDetailsByIdWithoutRemoveClause(int supervisorDelegateId, int adminId, int delegateUserId);
        //UPDATE DATA
        bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool RemoveSupervisorDelegateById(int supervisorDelegateId, int delegateUserId, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerifications(int selfAssessmentResultSupervisorVerificationId, string? comments, bool signedOff, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(int selfAssessmentResultSupervisorVerificationId);
        int RemoveSelfAssessmentResultSupervisorVerificationById(int id);
        bool RemoveCandidateAssessment(int candidateAssessmentId);
        void UpdateNotificationSent(int supervisorDelegateId);
        void UpdateCandidateAssessmentSupervisorVerificationById(int? candidateAssessmentSupervisorVerificationId, string? supervisorComments, bool signedOff);
        //INSERT DATA
        int AddSuperviseDelegate(int? supervisorAdminId, int? delegateUserId, string delegateEmail, string supervisorEmail, int centreId);
        int EnrolDelegateOnAssessment(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId, int centreId, bool isLoggedInUser);
        int InsertCandidateAssessmentSupervisor(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId);
        bool InsertSelfAssessmentResultSupervisorVerification(int candidateAssessmentSupervisorId, int resultId);
        //DELETE DATA
        bool RemoveCandidateAssessmentSupervisor(int selfAssessmentId, int supervisorDelegateId);
        int IsSupervisorDelegateExistAndReturnId(int? supervisorAdminId, string delegateEmail, int centreId);
        SupervisorDelegate GetSupervisorDelegateById(int supervisorDelegateId);
        void RemoveCandidateAssessmentSupervisorVerification(int id);
        bool RemoveDelegateSelfAssessmentsupervisor(int candidateAssessmentId, int supervisorDelegateId);
        void UpdateCandidateAssessmentNonReportable(int candidateAssessmentId);
    }
    public class SupervisorDataService : ISupervisorDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorDataService> logger;

        private const string supervisorDelegateDetailFields = @"
            sd.ID, sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateEmail, sd.DelegateUserID, sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Removed, sd.InviteHash, u.FirstName, u.LastName, jg.JobGroupName, da.ID AS DelegateID, da.Answer1, da.Answer2, da.Answer3, da.Answer4, 
             da.Answer5, da.Answer6, da.CandidateNumber, u.ProfessionalRegistrationNumber, u.PrimaryEmail AS CandidateEmail, cp1.CustomPrompt AS CustomPrompt1, cp2.CustomPrompt AS CustomPrompt2, cp3.CustomPrompt AS CustomPrompt3, 
             cp4.CustomPrompt AS CustomPrompt4, cp5.CustomPrompt AS CustomPrompt5, cp6.CustomPrompt AS CustomPrompt6, COALESCE (aa.CentreID, da.CentreID) AS CentreID, au.FirstName + ' ' + au.LastName AS SupervisorName,
                 (SELECT COUNT(ca.ID) AS Expr1
            FROM CandidateAssessments AS ca INNER JOIN SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID LEFT JOIN CandidateAssessmentSupervisors AS cas ON ca.ID = cas.CandidateAssessmentID 
            WHERE (ca.DelegateUserID = sd.DelegateUserID) AND (ca.RemovedDate IS NULL) AND (cas.SupervisorDelegateId = sd.ID  OR (cas.CandidateAssessmentID IS NULL AND ca.CentreID = aa.CentreID AND sa.[National] = 1))) AS CandidateAssessmentCount, CAST(COALESCE (au2.IsNominatedSupervisor, 0) AS Bit) AS DelegateIsNominatedSupervisor, CAST(COALESCE (au2.IsSupervisor, 0) AS Bit)
             AS DelegateIsSupervisor ";
        private const string supervisorDelegateDetailTables = @"
            CustomPrompts AS cp2 
            RIGHT OUTER JOIN CustomPrompts AS cp3 
            RIGHT OUTER JOIN CustomPrompts AS cp4 
            RIGHT OUTER JOIN CustomPrompts AS cp5 
            RIGHT OUTER JOIN CustomPrompts AS cp6 
            RIGHT OUTER JOIN Centres AS ct 
	            ON cp6.CustomPromptID = ct.CustomField6PromptID 
            ON cp5.CustomPromptID = ct.CustomField5PromptID 
            ON cp4.CustomPromptID = ct.CustomField4PromptID 
            ON cp3.CustomPromptID = ct.CustomField3PromptID 
            ON cp2.CustomPromptID = ct.CustomField2PromptID 
            LEFT OUTER JOIN CustomPrompts AS cp1 
	            ON ct.CustomField1PromptID = cp1.CustomPromptID 
            FULL OUTER JOIN DelegateAccounts AS da 
            INNER JOIN Users AS u 
	            ON da.UserID = u.ID 
            ON ct.CentreID = da.CentreID 
            RIGHT OUTER JOIN JobGroups AS jg 
	            ON u.JobGroupID = jg.JobGroupID 			 
            FULL OUTER JOIN Users AS du 
            INNER JOIN AdminAccounts AS au2 
	            ON du.ID = au2.UserID 
            ON da.CentreID = au2.CentreID AND da.UserID = du.ID 			 
            FULL OUTER JOIN SupervisorDelegates AS sd 
            INNER JOIN AdminAccounts AS aa 
	            ON sd.SupervisorAdminID = aa.ID 
            ON da.CentreID = aa.CentreID 
            AND u.ID = sd.DelegateUserID
            INNER JOIN Users AS au ON  aa.UserID = au.ID
            ";

        private const string delegateSelfAssessmentFields = "ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, COALESCE (sasr.RoleName, 'Supervisor') AS SupervisorRoleTitle, ca.StartedDate";
        private const string signedOffFields = @"(SELECT TOP (1) casv.Verified
FROM CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID
WHERE(cas.CandidateAssessmentID = ca.ID) AND(casv.Requested IS NOT NULL) AND(casv.Verified IS NOT NULL)
ORDER BY casv.Requested DESC) AS SignedOffDate,
(SELECT TOP(1) casv.SignedOff
FROM   CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID
WHERE(cas.CandidateAssessmentID = ca.ID) AND(casv.Requested IS NOT NULL) AND(casv.Verified IS NOT NULL)
ORDER BY casv.Requested DESC) AS SignedOff,";

        public SupervisorDataService(IDbConnection connection, ILogger<SupervisorDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public DashboardData? GetDashboardDataForAdminId(int adminId)
        {
            return connection.Query<DashboardData>(
                @"SELECT (SELECT COUNT(sd.ID) AS StaffCount
                        FROM   CustomPrompts AS cp6 
	                    RIGHT OUTER JOIN CustomPrompts AS cp5 
	                    RIGHT OUTER JOIN DelegateAccounts AS da
	                    RIGHT OUTER JOIN SupervisorDelegates AS sd 
	                    INNER JOIN AdminUsers AS au 
		                    ON sd.SupervisorAdminID = au.AdminID 
	                    INNER JOIN Centres AS ct 
		                    ON au.CentreID = ct.CentreID 
	                    ON da.CentreID = ct.CentreID 
		                    AND da.UserID = sd.DelegateUserID 
	                    LEFT OUTER JOIN Users AS u 
	                    LEFT OUTER JOIN JobGroups AS jg 
		                    ON u.JobGroupID = jg.JobGroupID
	                    ON da.UserID = u.ID 
	                    LEFT OUTER JOIN CustomPrompts AS cp1 
		                    ON ct.CustomField1PromptID = cp1.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp2 
		                    ON ct.CustomField2PromptID = cp2.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp3 
		                    ON ct.CustomField3PromptID = cp3.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp4 
		                    ON ct.CustomField4PromptID = cp4.CustomPromptID 
	                    ON cp5.CustomPromptID = ct.CustomField5PromptID 
	                    ON cp6.CustomPromptID = ct.CustomField6PromptID 
	                    LEFT OUTER JOIN AdminAccounts AS au2 
		                    ON da.UserID = au2.UserID AND da.CentreID = au2.CentreID
                    WHERE (sd.SupervisorAdminID = @adminId) AND (sd.Removed IS NULL) AND 
                     (u.ID = da.UserID OR sd.DelegateUserID IS NULL)) AS StaffCount,
                         (SELECT COUNT(ID) AS StaffCount
                         FROM SupervisorDelegates AS SupervisorDelegates_1
                         WHERE (SupervisorAdminID = @adminId)
                            AND (DelegateUserID IS NULL)
                            AND (Removed IS NULL)) AS StaffUnregisteredCount,
                    (SELECT COUNT(ca.ID) AS ProfileSelfAssessmentCount
                         FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                           CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                             SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID
                    WHERE (sd.SupervisorAdminID = @adminId) AND (cas.Removed IS NULL) AND ((ca.RemovedDate IS NULL))) AS ProfileSelfAssessmentCount,
                    (SELECT COUNT(DISTINCT sa.ID) AS Expr1
                        FROM   SelfAssessments AS sa INNER JOIN
                        CandidateAssessments AS ca ON sa.ID = ca.SelfAssessmentID LEFT OUTER JOIN
                        SupervisorDelegates AS sd INNER JOIN
                        CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId ON ca.ID = cas.CandidateAssessmentID
                        WHERE (sd.SupervisorAdminID = @adminId)) As ProfileCount,
                        COALESCE
                        ((SELECT COUNT(casv.ID) AS Expr1
                        FROM    CandidateAssessmentSupervisors AS cas INNER JOIN
                        CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                        SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                        CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
                        WHERE (sd.SupervisorAdminID = @adminId) AND ((ca.RemovedDate IS NULL) AND (cas.Removed IS NULL)) AND (casv.Verified IS NULL)
                        ), 0) AS AwaitingReviewCount", new { adminId }
                ).FirstOrDefault();
        }

        public IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId, int? adminIdCategoryID)
        {
            return connection.Query<SupervisorDelegateDetail>(
                $@"SELECT sd.ID, 
		                sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateEmail, sd.DelegateUserID,da.Active,
                        sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Removed, sd.InviteHash, 
		                u.FirstName, u.LastName, u.ProfessionalRegistrationNumber, u.PrimaryEmail AS CandidateEmail,
		                jg.JobGroupName, 
		                da.Answer1, da.Answer2, da.Answer3, da.Answer4, da.Answer5, da.Answer6, da.CandidateNumber, 
		                cp1.CustomPrompt AS CustomPrompt1, cp2.CustomPrompt AS CustomPrompt2, 
		                cp3.CustomPrompt AS CustomPrompt3, cp4.CustomPrompt AS CustomPrompt4,             
		                cp5.CustomPrompt AS CustomPrompt5, cp6.CustomPrompt AS CustomPrompt6, 
		                COALESCE (au.CentreID, da.CentreID) AS CentreID, 
		                au.Forename + ' ' + au.Surname AS SupervisorName,                 
		                (SELECT COUNT(ca.ID) AS Expr1
                        FROM CandidateAssessments AS ca  LEFT JOIN
                        CandidateAssessmentSupervisors AS cas ON cas.CandidateAssessmentID = ca.ID AND cas.Removed IS NULL AND cas.SupervisorDelegateId = sd.ID INNER JOIN
                        SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID 
                        WHERE (ca.RemovedDate IS NULL) AND (ca.DelegateUserID=sd.DelegateUserID) AND (cas.SupervisorDelegateId = sd.ID OR (cas.CandidateAssessmentID IS NULL)
				        AND ((sa.SupervisorSelfAssessmentReview = 1) OR (sa.SupervisorResultsReview = 1)))
                        AND (ISNULL(@adminIdCategoryID, 0) = 0 OR sa.CategoryID = @adminIdCategoryID)
                            ) AS CandidateAssessmentCount, 
		                CAST(COALESCE (au2.IsNominatedSupervisor, 0) AS Bit) AS DelegateIsNominatedSupervisor, 
		                CAST(COALESCE (au2.IsSupervisor, 0) AS Bit) AS DelegateIsSupervisor,             
		                da.ID AS Expr1
                    FROM   CustomPrompts AS cp6 
	                    RIGHT OUTER JOIN CustomPrompts AS cp5 
	                    RIGHT OUTER JOIN DelegateAccounts AS da
	                    RIGHT OUTER JOIN SupervisorDelegates AS sd 
	                    INNER JOIN AdminUsers AS au 
		                    ON sd.SupervisorAdminID = au.AdminID 
	                    INNER JOIN Centres AS ct 
		                    ON au.CentreID = ct.CentreID 
	                    ON da.CentreID = ct.CentreID 
		                    AND da.UserID = sd.DelegateUserID 
	                    LEFT OUTER JOIN Users AS u 
	                    LEFT OUTER JOIN JobGroups AS jg 
		                    ON u.JobGroupID = jg.JobGroupID
	                    ON da.UserID = u.ID 
	                    LEFT OUTER JOIN CustomPrompts AS cp1 
		                    ON ct.CustomField1PromptID = cp1.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp2 
		                    ON ct.CustomField2PromptID = cp2.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp3 
		                    ON ct.CustomField3PromptID = cp3.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp4 
		                    ON ct.CustomField4PromptID = cp4.CustomPromptID 
	                    ON cp5.CustomPromptID = ct.CustomField5PromptID 
	                    ON cp6.CustomPromptID = ct.CustomField6PromptID 
	                    LEFT OUTER JOIN AdminAccounts AS au2 
		                    ON da.UserID = au2.UserID AND da.CentreID = au2.CentreID
                    WHERE (sd.SupervisorAdminID = @adminId) AND (sd.Removed IS NULL) AND 
                     (u.ID = da.UserID OR sd.DelegateUserID IS NULL)
                    ORDER BY u.LastName, COALESCE (u.FirstName, sd.DelegateEmail)
                    ", new { adminId, adminIdCategoryID }
                );
        }

        public IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminIdWithoutRemovedClause(int adminId)
        {
            return connection.Query<SupervisorDelegateDetail>(
                $@"SELECT sd.ID, 
		                sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateEmail, sd.DelegateUserID,da.Active,
                        sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Removed, sd.InviteHash, 
		                u.FirstName, u.LastName, u.ProfessionalRegistrationNumber, u.PrimaryEmail AS CandidateEmail,
		                jg.JobGroupName, 
		                da.Answer1, da.Answer2, da.Answer3, da.Answer4, da.Answer5, da.Answer6, da.CandidateNumber, 
		                cp1.CustomPrompt AS CustomPrompt1, cp2.CustomPrompt AS CustomPrompt2, 
		                cp3.CustomPrompt AS CustomPrompt3, cp4.CustomPrompt AS CustomPrompt4,             
		                cp5.CustomPrompt AS CustomPrompt5, cp6.CustomPrompt AS CustomPrompt6, 
		                COALESCE (au.CentreID, da.CentreID) AS CentreID, 
		                au.Forename + ' ' + au.Surname AS SupervisorName,                 
		                (SELECT COUNT(ca.ID) AS Expr1
                        FROM CandidateAssessments AS ca  LEFT JOIN
                        CandidateAssessmentSupervisors AS cas ON cas.CandidateAssessmentID = ca.ID AND cas.Removed IS NULL AND cas.SupervisorDelegateId = sd.ID INNER JOIN
                        SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID 
                        WHERE (ca.RemovedDate IS NULL) AND (ca.DelegateUserID=sd.DelegateUserID) AND (cas.SupervisorDelegateId = sd.ID OR (cas.CandidateAssessmentID IS NULL)
				        AND ((sa.SupervisorSelfAssessmentReview = 1) OR (sa.SupervisorResultsReview = 1)))) AS CandidateAssessmentCount, 
		                CAST(COALESCE (au2.IsNominatedSupervisor, 0) AS Bit) AS DelegateIsNominatedSupervisor, 
		                CAST(COALESCE (au2.IsSupervisor, 0) AS Bit) AS DelegateIsSupervisor,             
		                da.ID AS Expr1
                    FROM   CustomPrompts AS cp6 
	                    RIGHT OUTER JOIN CustomPrompts AS cp5 
	                    RIGHT OUTER JOIN DelegateAccounts AS da
	                    RIGHT OUTER JOIN SupervisorDelegates AS sd 
	                    INNER JOIN AdminUsers AS au 
		                    ON sd.SupervisorAdminID = au.AdminID 
	                    INNER JOIN Centres AS ct 
		                    ON au.CentreID = ct.CentreID 
	                    ON da.CentreID = ct.CentreID 
		                    AND da.UserID = sd.DelegateUserID 
	                    LEFT OUTER JOIN Users AS u 
	                    LEFT OUTER JOIN JobGroups AS jg 
		                    ON u.JobGroupID = jg.JobGroupID
	                    ON da.UserID = u.ID 
	                    LEFT OUTER JOIN CustomPrompts AS cp1 
		                    ON ct.CustomField1PromptID = cp1.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp2 
		                    ON ct.CustomField2PromptID = cp2.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp3 
		                    ON ct.CustomField3PromptID = cp3.CustomPromptID 
	                    LEFT OUTER JOIN CustomPrompts AS cp4 
		                    ON ct.CustomField4PromptID = cp4.CustomPromptID 
	                    ON cp5.CustomPromptID = ct.CustomField5PromptID 
	                    ON cp6.CustomPromptID = ct.CustomField6PromptID 
	                    LEFT OUTER JOIN AdminAccounts AS au2 
		                    ON da.UserID = au2.UserID AND da.CentreID = au2.CentreID
                    WHERE (sd.SupervisorAdminID = @adminId)  AND 
                     (u.ID = da.UserID OR sd.DelegateUserID IS NULL)
                    ORDER BY u.LastName, COALESCE (u.FirstName, sd.DelegateEmail)
                    ", new { adminId }
                );
        }
        public int AddSuperviseDelegate(int? supervisorAdminId, int? delegateUserId, string delegateEmail, string supervisorEmail, int centreId)
        {
            var addedByDelegate = (delegateUserId != null);
            if (delegateEmail.Length == 0 | supervisorEmail.Length == 0)
            {
                logger.LogWarning(
                    $"Not adding delegate to SupervisorDelegates as it failed server side validation. supervisorAdminId: {supervisorAdminId}, delegateEmail: {delegateEmail}"
                );
                return -3;
            }
            if (delegateUserId == null)
            {
                delegateUserId = (int?)connection.ExecuteScalar(
                    @"SELECT da.UserID AS DelegateUserID 
                            FROM Users u
                            INNER JOIN DelegateAccounts da
                            ON da.UserID = u.ID
	                        LEFT JOIN UserCentreDetails ucd
	                        ON ucd.UserID = u.ID
                            AND ucd.CentreID = da.CentreID
                            WHERE (ucd.Email = @delegateEmail OR u.PrimaryEmail = @delegateEmail)
                            AND da.CentreID = @centreId", new { delegateEmail, centreId });
            }

            int existingId = Convert.ToInt32(connection.ExecuteScalar(
                @"
                    SELECT COALESCE
                    ((SELECT Top 1 ID
                        FROM  SupervisorDelegates sd
                        WHERE ((sd.SupervisorAdminID = @supervisorAdminID) OR (sd.SupervisorAdminID > 0 AND SupervisorEmail = @supervisorEmail AND @supervisorAdminID = NULL))
		                      AND((sd.DelegateUserID = @delegateUserId) OR (sd.DelegateUserID > 0 AND DelegateEmail = @delegateEmail AND @delegateUserId = NULL)) ORDER BY sd.DelegateUserID 
                        ), 0) AS ID",
                new
                {
                    supervisorEmail,
                    delegateEmail,
                    supervisorAdminId = supervisorAdminId ?? 0,
                    delegateUserId = delegateUserId ?? 0
                }
            ));

            if (existingId > 0)
            {
                var numberOfAffectedRows = connection.Execute(@"UPDATE SupervisorDelegates SET Removed = NULL, DelegateUserId = @delegateUserId, 
                                                                DelegateEmail = @delegateEmail WHERE ID = @existingId", new { delegateUserId, delegateEmail, existingId });
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO SupervisorDelegates (SupervisorAdminID, DelegateEmail, DelegateUserID, SupervisorEmail, AddedByDelegate)
                    VALUES (@supervisorAdminId, @delegateEmail, @delegateUserId, @supervisorEmail, @addedByDelegate)",
                    new { supervisorAdminId, delegateEmail, delegateUserId, supervisorEmail, addedByDelegate });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting SupervisorDelegate as db insert failed. supervisorAdminId: {supervisorAdminId}, delegateEmail: {delegateEmail}, delegateUserId: {delegateUserId}"
                    );
                    return -1;
                }

                existingId = Convert.ToInt32(connection.ExecuteScalar(
                    @"
                    SELECT COALESCE
                    ((SELECT ID
                        FROM    SupervisorDelegates sd
                        WHERE(SupervisorEmail = @supervisorEmail) AND(DelegateEmail = @delegateEmail)
                            AND(sd.SupervisorAdminID = @supervisorAdminID OR @supervisorAdminID = 0)
                            AND(sd.DelegateUserID = @delegateUserId OR @delegateUserID = 0)
                            AND (sd.Removed IS NULL)
                        ), 0) AS ID",
                    new
                    {
                        supervisorEmail,
                        delegateEmail,
                        supervisorAdminId = supervisorAdminId ?? 0,
                        delegateUserId = delegateUserId ?? 0
                    }
                )); return existingId;
            }
        }

        public SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId, int adminId, int delegateUserId)
        {
            int delegateId = 0;
            var supervisorDelegateDetail = connection.Query<SupervisorDelegateDetail>(
                $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.ID = @supervisorDelegateId) AND (sd.DelegateUserID = @delegateUserId OR sd.SupervisorAdminID = @adminId) AND (Removed IS NULL)", new { supervisorDelegateId, adminId, delegateUserId }
            ).FirstOrDefault();

            if (supervisorDelegateDetail != null && supervisorDelegateDetail.DelegateID != null)
            {
                delegateId = (int)supervisorDelegateDetail!.DelegateID!;
            }

            if (delegateUserId == 0)
            {
                if (supervisorDelegateDetail != null && supervisorDelegateDetail.DelegateUserID != null)
                {
                    delegateUserId = (int)supervisorDelegateDetail!.DelegateUserID!;
                }
            }

            var delegateDetails = connection.Query<SupervisorDelegateDetail>(
               $@"SELECT u.ID AS DelegateUserId, u.FirstName, u.LastName, u.ProfessionalRegistrationNumber, u.PrimaryEmail AS CandidateEmail, da.CandidateNumber
                    FROM   Users u
                    INNER JOIN DelegateAccounts da
                        ON da.UserID = u.ID
                    WHERE u.ID = @delegateUserId AND u.Active = 1 AND da.Active = 1 AND da.ID = @delegateId", new { delegateUserId, delegateId }
            ).FirstOrDefault();

            if (supervisorDelegateDetail != null && delegateDetails != null)
            {
                supervisorDelegateDetail.DelegateUserID = delegateUserId;
                supervisorDelegateDetail.FirstName = delegateDetails.FirstName;
                supervisorDelegateDetail.LastName = delegateDetails.LastName;
                supervisorDelegateDetail.ProfessionalRegistrationNumber = delegateDetails.ProfessionalRegistrationNumber;
                supervisorDelegateDetail.CandidateEmail = delegateDetails.CandidateEmail;
                supervisorDelegateDetail.CandidateNumber = delegateDetails.CandidateNumber;
            }

            return supervisorDelegateDetail!;
        }

        public SupervisorDelegateDetail GetSupervisorDelegateDetailsByIdWithoutRemoveClause(int supervisorDelegateId, int adminId, int delegateUserId)
        {
            int delegateId = 0;
            var supervisorDelegateDetail = connection.Query<SupervisorDelegateDetail>(
                $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.ID = @supervisorDelegateId) AND (sd.DelegateUserID = @delegateUserId OR sd.SupervisorAdminID = @adminId)", new { supervisorDelegateId, adminId, delegateUserId }
            ).FirstOrDefault();

            if (supervisorDelegateDetail != null && supervisorDelegateDetail.DelegateID != null)
            {
                delegateId = (int)supervisorDelegateDetail!.DelegateID!;
            }

            if (delegateUserId == 0)
            {
                if (supervisorDelegateDetail != null && supervisorDelegateDetail.DelegateUserID != null)
                {
                    delegateUserId = (int)supervisorDelegateDetail!.DelegateUserID!;
                }
            }

            var delegateDetails = connection.Query<SupervisorDelegateDetail>(
               $@"SELECT u.ID AS DelegateUserId, u.FirstName, u.LastName, u.ProfessionalRegistrationNumber, u.PrimaryEmail AS CandidateEmail, da.CandidateNumber
                    FROM   Users u
                    INNER JOIN DelegateAccounts da
                        ON da.UserID = u.ID
                    WHERE u.ID = @delegateUserId AND u.Active = 1 AND da.Active = 1 AND da.ID = @delegateId", new { delegateUserId, delegateId }
            ).FirstOrDefault();

            if (supervisorDelegateDetail != null && delegateDetails != null)
            {
                supervisorDelegateDetail.DelegateUserID = delegateUserId;
                supervisorDelegateDetail.FirstName = delegateDetails.FirstName;
                supervisorDelegateDetail.LastName = delegateDetails.LastName;
                supervisorDelegateDetail.ProfessionalRegistrationNumber = delegateDetails.ProfessionalRegistrationNumber;
                supervisorDelegateDetail.CandidateEmail = delegateDetails.CandidateEmail;
                supervisorDelegateDetail.CandidateNumber = delegateDetails.CandidateNumber;
            }

            return supervisorDelegateDetail!;
        }

        public SupervisorDelegate GetSupervisorDelegate(int adminId, int delegateUserId)
        {
            var supervisorDelegateDetail = connection.Query<SupervisorDelegate>(
                $@"SELECT ID,SupervisorAdminID,DelegateEmail,Added,NotificationSent,Removed,
                    SupervisorEmail,AddedByDelegate,InviteHash,DelegateUserID
                    FROM SupervisorDelegates
                    WHERE DelegateUserID = @delegateUserId AND SupervisorAdminID = @adminId ", new { adminId, delegateUserId }
            ).FirstOrDefault();

            return supervisorDelegateDetail!;
        }

        public int? ValidateDelegate(int centreId, string delegateEmail)
        {
            int? delegateUserId = (int?)connection.ExecuteScalar(
                 @"SELECT TOP 1 da.UserID AS DelegateUserID 
                            FROM Users u
                            INNER JOIN DelegateAccounts da
                            ON da.UserID = u.ID
                            LEFT JOIN UserCentreDetails ucd
                            ON ucd.UserID = u.ID
                            WHERE (u.PrimaryEmail = @delegateEmail
                            OR ucd.Email = @delegateEmail)
                            AND u.Active = 1 
                            AND da.CentreID = @centreId", new { delegateEmail, centreId });

            if (delegateUserId != null && delegateUserId > 0)
            {
                int? delegateId = (int?)connection.ExecuteScalar(
                     @"SELECT da.ID FROM DelegateAccounts da
                            WHERE da.UserID=@delegateUserId 
                            AND da.Approved = 1
                            AND da.CentreID = @centreId", new { delegateUserId, centreId });
                return delegateId;
            }
            else
            {
                return 0;
            }
        }

        public IEnumerable<SupervisorForEnrolDelegate> GetSupervisorForEnrolDelegate(int CentreID, int CategoryID)
        {
            return connection.Query<SupervisorForEnrolDelegate>(
                $@"SELECT aa.ID AS AdminID,
                        u.FirstName + ' ' +  u.LastName + ' (' + COALESCE(ucd.Email, u.PrimaryEmail) +')' AS Name, 
			            COALESCE(ucd.Email, u.PrimaryEmail) AS Email 
				FROM  AdminAccounts AS aa INNER JOIN
                                Users AS u ON aa.UserID = u.ID INNER JOIN
                                Centres AS c ON aa.CentreID = c.CentreID LEFT OUTER JOIN
                                UserCentreDetails AS ucd ON u.ID = ucd.UserID AND c.CentreID = ucd.CentreID
                WHERE (aa.IsSupervisor = 1) AND (c.CentreID = @CentreID) AND 
						(ISNULL(aa.CategoryID, 0) = 0 OR aa.CategoryID = @CategoryID) AND 
							(aa.Active = 1)
				GROUP BY aa.ID, u.LastName, u.FirstName, COALESCE(ucd.Email, u.PrimaryEmail), CentreName
				ORDER BY u.FirstName, u.LastName",
                new { CentreID, CategoryID });
        }

        public bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int delegateUserId, int adminId)
        {
            var numberOfAffectedRows = connection.Execute(
         @"UPDATE SupervisorDelegates SET Confirmed = getUTCDate()
            WHERE ID = @supervisorDelegateId AND Confirmed IS NULL AND Removed IS NULL AND (CandidateID = @candidateId OR SupervisorAdminID = @adminId)",
        new { supervisorDelegateId, delegateUserId, adminId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not confirming SupervisorDelegate as db update failed. supervisorDelegateId: {supervisorDelegateId}, delegateUserId: {delegateUserId}, adminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public bool RemoveSupervisorDelegateById(int supervisorDelegateId, int delegateUserId, int adminId)
        {

            connection.Execute(
                @"DELETE FROM sarsv FROM SelfAssessmentResultSupervisorVerifications as sarsv
                    LEFT JOIN CandidateAssessmentSupervisors AS cas ON cas.ID = sarsv.CandidateAssessmentSupervisorID
                    WHERE cas.SupervisorDelegateId = @supervisorDelegateId AND cas.Removed IS NULL AND sarsv.Verified IS NULL", new { supervisorDelegateId }
            );

            connection.Execute(
                @"DELETE FROM casv FROM CandidateAssessmentSupervisorVerifications casv INNER JOIN
			                CandidateAssessmentSupervisors cas ON casv.CandidateAssessmentSupervisorID = cas.ID
                    WHERE cas.SupervisorDelegateId = @supervisorDelegateId
			                AND casv.Verified IS NULL AND casv.SignedOff = 0", new { supervisorDelegateId }
            );

            var numberOfAffectedRows = connection.Execute(
         @"UPDATE SupervisorDelegates SET Removed = getUTCDate()
            WHERE ID = @supervisorDelegateId AND Removed IS NULL AND (DelegateUserID = @delegateUserId OR SupervisorAdminID = @adminId)",
        new { supervisorDelegateId, delegateUserId, adminId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not removing SupervisorDelegate as db update failed. supervisorDelegateId: {supervisorDelegateId}, delegateUserId: {delegateUserId}, adminId: {adminId}"
                );
                return false;
            }

            var numberOfAffectedRowsCAS = connection.Execute(
         @"UPDATE CandidateAssessmentSupervisors SET Removed = getUTCDate()
            WHERE SupervisorDelegateId = @supervisorDelegateId AND Removed IS NULL",
        new { supervisorDelegateId });
            if (numberOfAffectedRowsCAS < 1)
            {
                logger.LogWarning(
                    $"Not removing CandidateAssessmentSupervisors as db update failed. supervisorDelegateId: {supervisorDelegateId}"
                );
                return false;
            }
            return true;
        }

        public IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int? adminIdCategoryId)
        {
            return connection.Query<DelegateSelfAssessment>(
                @$"SELECT {delegateSelfAssessmentFields}, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, r.RoleProfile, sg.SubGroup, pg.ProfessionalGroup,CONVERT(BIT, IIF(cas.CandidateAssessmentID IS NULL, 0, 1)) AS IsAssignedToSupervisor,ca.DelegateUserID,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
                 FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL) AND (sarsv.Superceded = 0)) AS ResultsVerificationRequests
                 FROM   CandidateAssessments AS ca  LEFT JOIN
                 CandidateAssessmentSupervisors AS cas ON cas.CandidateAssessmentID = ca.ID AND cas.Removed IS NULL and cas.SupervisorDelegateId=@supervisorDelegateId INNER JOIN
                 SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID LEFT OUTER JOIN
                 NRPProfessionalGroups AS pg ON sa.NRPProfessionalGroupID = pg.ID LEFT OUTER JOIN
                 NRPSubGroups AS sg ON sa.NRPSubGroupID = sg.ID LEFT OUTER JOIN
                 NRPRoles AS r ON sa.NRPRoleID = r.ID
                 LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                 RIGHT OUTER JOIN SupervisorDelegates AS sd ON sd.ID=@supervisorDelegateId
                 RIGHT OUTER JOIN AdminAccounts AS au ON au.ID = sd.SupervisorAdminID
                 WHERE (ca.RemovedDate IS NULL) AND (ca.DelegateUserID=sd.DelegateUserID) AND (cas.SupervisorDelegateId = @supervisorDelegateId OR (cas.CandidateAssessmentID IS NULL)  AND ((sa.SupervisorSelfAssessmentReview = 1) OR
                       (sa.SupervisorResultsReview = 1))) AND
                       (ISNULL(@adminIdCategoryId, 0) = 0 OR sa.CategoryID = @adminIdCategoryId) ", new { supervisorDelegateId, adminIdCategoryId }
                );
        }
        public DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(int selfAssessmentId, int supervisorDelegateId)
        {
            return connection.Query<DelegateSelfAssessment>(
               @$"SELECT {delegateSelfAssessmentFields}, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
                    FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL) AND (sarsv.Superceded = 0)) AS ResultsVerificationRequests
                FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID
                 LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                WHERE  (ca.RemovedDate IS NULL) AND (cas.SupervisorDelegateId = @supervisorDelegateId)  AND (cas.Removed IS NULL) AND (sa.ID = @selfAssessmentId)", new { selfAssessmentId, supervisorDelegateId }
               ).FirstOrDefault();
        }
        public DelegateSelfAssessment? GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId, int? adminIdCategoryId)
        {
            return connection.Query<DelegateSelfAssessment>(
               @$"SELECT ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.QuestionLabel, sa.DescriptionLabel, sa.ReviewerCommentsLabel,
                            sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, ca.StartedDate,
                            COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed,
                            ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate,
                            (SELECT COUNT(*) AS Expr1
                                FROM    CandidateAssessmentSupervisorVerifications AS casv
                                WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                            {signedOffFields}
                            (SELECT COUNT(*) AS Expr1
                                FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                                WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL) AND (sarsv.Superceded = 0)) AS ResultsVerificationRequests,
                            sa.Vocabulary
                    FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                            CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                            SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID
                    WHERE (ca.ID = @candidateAssessmentId) AND (ISNULL(@adminIdCategoryID, 0) = 0 OR sa.CategoryID = @adminIdCategoryId)", new { candidateAssessmentId, adminIdCategoryId }
               ).FirstOrDefault();
        }
        public DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(int candidateAssessmentId, int supervisorDelegateId)
        {
            return connection.Query<DelegateSelfAssessment>(
               @$"SELECT {delegateSelfAssessmentFields}, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
                    FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL) AND (sarsv.Superceded = 0)) AS ResultsVerificationRequests
                FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID
                 LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                WHERE  (ca.RemovedDate IS NULL) AND (cas.SupervisorDelegateId = @supervisorDelegateId)  AND (cas.Removed IS NULL) AND (ca.ID = @candidateAssessmentId)", new { candidateAssessmentId, supervisorDelegateId }
               ).FirstOrDefault();
        }
        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedSignOffs(int adminId)
        {
            return connection.Query<SupervisorDashboardToDoItem>(
                @"SELECT ca.ID, sd.ID AS SupervisorDelegateId, u.FirstName + ' ' + u.LastName AS DelegateName, sa.Name AS ProfileName, casv.Requested, 1 AS SignOffRequest, 0 AS ResultsReviewRequest                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                    CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                    SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                    CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID INNER JOIN
                    Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
					AdminAccounts As aa ON sd.SupervisorAdminID = aa.ID
                WHERE (sd.SupervisorAdminID = @adminId) AND (casv.Verified IS NULL) AND (cas.Removed IS NULL)
                        AND (sd.Removed IS NULL)
                        AND (aa.CategoryID is null or sa.CategoryID = aa.CategoryID)", new { adminId }
                );
        }
        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedReviews(int adminId)
        {
            return connection.Query<SupervisorDashboardToDoItem>(
                @"SELECT ca.ID, sd.ID AS SupervisorDelegateId, u.FirstName + ' ' + u.LastName AS DelegateName, sa.Name AS ProfileName, MAX(sasv.Requested) AS Requested, 0 AS SignOffRequest, 1 AS ResultsReviewRequest                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                    CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                    Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                    SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
					SelfAssessmentResults AS sar ON sar.SelfAssessmentID = sa.ID INNER JOIN
					Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN					
                    SelfAssessmentResultSupervisorVerifications AS sasv ON sasv.SelfAssessmentResultId = sar.ID AND sasv.Superceded = 0
                        AND sasv.CandidateAssessmentSupervisorID = cas.ID AND sar.DateTime = (
						    SELECT TOP 1 sar2.DateTime
						    FROM SelfAssessmentResults AS sar2
						    WHERE sar2.ID = sar.ID AND sar2.SelfAssessmentID = sar.SelfAssessmentID AND sar2.CompetencyID = co.ID AND sar2.Result != 0 ORDER BY sar2.ID DESC
					) INNER JOIN
		            AdminAccounts AS aa ON sd.SupervisorAdminID = aa.ID
                WHERE (sd.SupervisorAdminID = @adminId) AND (cas.Removed IS NULL) AND (sasv.Verified IS NULL) AND (sd.Removed IS NULL)
                        AND (aa.CategoryID is null or sa.CategoryID = aa.CategoryID)
				GROUP BY sa.ID, ca.ID, sd.ID, u.FirstName, u.LastName, sa.Name,cast(sasv.Requested as date)", new { adminId }
                );
        }

        public DelegateSelfAssessment? GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId, int? adminIdCategoryId)
        {
            return connection.Query<DelegateSelfAssessment>(
                @$"SELECT ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, sa.ReviewerCommentsLabel, COALESCE (sasr.RoleName, 'Supervisor') AS SupervisorRoleTitle, ca.StartedDate, ca.LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, r.RoleProfile, sg.SubGroup, pg.ProfessionalGroup, sa.SupervisorResultsReview AS IsSupervisorResultsReviewed,
                            (SELECT COUNT(*) AS Expr1
                            FROM    CandidateAssessmentSupervisorVerifications AS casv
                            WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                            {signedOffFields}
                            (SELECT COUNT(*) AS Expr1
                            FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                            WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL) AND (Superceded = 0)) AS ResultsVerificationRequests,
                            ca.NonReportable,ca.DelegateUserID,
                            sa.Vocabulary
                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                            CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                            SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID INNER JOIN
                            SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID LEFT OUTER JOIN
                            NRPProfessionalGroups AS pg ON sa.NRPProfessionalGroupID = pg.ID LEFT OUTER JOIN
                            NRPSubGroups AS sg ON sa.NRPSubGroupID = sg.ID LEFT OUTER JOIN
                            NRPRoles AS r ON sa.NRPRoleID = r.ID
                            LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                    WHERE (ca.ID = @candidateAssessmentId) AND (cas.Removed IS NULL) AND (sd.SupervisorAdminID = @adminId) AND (ISNULL(@adminIdCategoryID, 0) = 0 OR sa.CategoryID = @adminIdCategoryId)",
                new { candidateAssessmentId, adminId, adminIdCategoryId }
                ).FirstOrDefault();
        }
        public bool UpdateSelfAssessmentResultSupervisorVerifications(int selfAssessmentResultSupervisorVerificationId, string? comments, bool signedOff, int adminId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentResultSupervisorVerifications
                    SET       Verified = getUTCDate(), Comments = @comments, SignedOff = @signedOff
                    FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
                         CandidateAssessmentSupervisors AS cas ON SelfAssessmentResultSupervisorVerifications.CandidateAssessmentSupervisorID = cas.ID INNER JOIN
                         SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID
                    WHERE SelfAssessmentResultSupervisorVerifications.ID = @selfAssessmentResultSupervisorVerificationId AND sd.SupervisorAdminID = @adminId",
                new { selfAssessmentResultSupervisorVerificationId, comments, signedOff, adminId }
                );
            if (numberOfAffectedRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(int selfAssessmentResultSupervisorVerificationId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE SelfAssessmentResultSupervisorVerifications
                    SET   EmailSent = getUTCDate()
                    FROM  SelfAssessmentResultSupervisorVerifications 
                    WHERE ID = @selfAssessmentResultSupervisorVerificationId",
                new { selfAssessmentResultSupervisorVerificationId }
                );
            return numberOfAffectedRows > 0;
        }

        public int RemoveSelfAssessmentResultSupervisorVerificationById(int id)
        {
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM SelfAssessmentResultSupervisorVerifications WHERE ID = @id",
                new { id });

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not deleting supervisor verifications as db update failed. SelfAssessmentResultSupervisorVerification.Id: {id}"
                );
            }
            return numberOfAffectedRows;
        }
        public IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int delegateUserId, int centreId, int? categoryId)
        {
            return connection.Query<RoleProfile>(
                $@"SELECT rp.ID, rp.Name AS RoleProfileName, rp.Description, rp.BrandID, rp.ParentSelfAssessmentID, rp.[National], rp.[Public], rp.CreatedByAdminID AS OwnerAdminID, rp.NRPProfessionalGroupID, rp.NRPSubGroupID, rp.NRPRoleID, rp.PublishStatusID, 0 AS UserRole, rp.CreatedDate,
                            (SELECT BrandName
                             FROM    Brands
                             WHERE (BrandID = rp.BrandID)) AS Brand, '' AS ParentSelfAssessment, '' AS Owner, rp.Archived, rp.LastEdit,
                             (SELECT ProfessionalGroup
                             FROM    NRPProfessionalGroups
                             WHERE (ID = rp.NRPProfessionalGroupID)) AS NRPProfessionalGroup,
                            (SELECT SubGroup
                             FROM    NRPSubGroups
                             WHERE (ID = rp.NRPSubGroupID)) AS NRPSubGroup,
                             (SELECT RoleProfile
                             FROM    NRPRoles
                             WHERE (ID = rp.NRPRoleID)) AS NRPRole, 0 AS SelfAssessmentReviewID
                FROM   SelfAssessments AS rp INNER JOIN
                         CentreSelfAssessments AS csa ON rp.ID = csa.SelfAssessmentID AND csa.CentreID = @centreId
                WHERE (rp.ArchivedDate IS NULL) AND (rp.ID NOT IN
                             (SELECT SelfAssessmentID
                             FROM    CandidateAssessments AS CA
                             WHERE (DelegateUserID = @delegateUserId) AND (RemovedDate IS NULL)
                                AND (CompletedDate IS NULL)))
                        AND ((rp.SupervisorSelfAssessmentReview = 1) OR (rp.SupervisorResultsReview = 1))
                        AND (ISNULL(@categoryId, 0) = 0 OR rp.CategoryID = @categoryId)", new { delegateUserId, centreId, categoryId }
                );
        }

        public RoleProfile? GetRoleProfileById(int selfAssessmentId)
        {
            return connection.Query<RoleProfile>(
                $@"SELECT ID, Name AS RoleProfileName, Description, BrandID, ParentSelfAssessmentID, [National], [Public], CreatedByAdminID AS OwnerAdminID, NRPProfessionalGroupID, NRPSubGroupID, NRPRoleID, PublishStatusID, 0 AS UserRole, CreatedDate,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = rp.BrandID)) AS Brand,
                 '' AS ParentSelfAssessment,
                 '' AS Owner, Archived, LastEdit,
                 (SELECT ProfessionalGroup
                 FROM    NRPProfessionalGroups
                 WHERE (ID = rp.NRPProfessionalGroupID)) AS NRPProfessionalGroup,
                 (SELECT SubGroup
                 FROM    NRPSubGroups
                 WHERE (ID = rp.NRPSubGroupID)) AS NRPSubGroup,
                 (SELECT RoleProfile
                 FROM    NRPRoles
                 WHERE (ID = rp.NRPRoleID)) AS NRPRole, 0 AS SelfAssessmentReviewID
                 FROM   SelfAssessments AS rp 
                 WHERE (ID = @selfAssessmentId)", new { selfAssessmentId }
                ).FirstOrDefault();
        }

        public IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesForSelfAssessment(int selfAssessmentId)
        {
            return connection.Query<SelfAssessmentSupervisorRole>(
               $@"SELECT ID, SelfAssessmentID, RoleName, RoleDescription, SelfAssessmentReview, ResultsReview,AllowSupervisorRoleSelection
                  FROM   SelfAssessmentSupervisorRoles
                  WHERE (SelfAssessmentID = @selfAssessmentId) 
                  ORDER BY RoleName", new { selfAssessmentId }
               );
        }

        public IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesBySelfAssessmentIdForSupervisor(int selfAssessmentId)
        {
            return connection.Query<SelfAssessmentSupervisorRole>(
               $@"SELECT ID, SelfAssessmentID, RoleName, RoleDescription, SelfAssessmentReview, ResultsReview
                  FROM   SelfAssessmentSupervisorRoles
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (AllowSupervisorRoleSelection = 1)
                  ORDER BY RoleName", new { selfAssessmentId }
               );
        }
        public IEnumerable<SelfAssessmentSupervisorRole> GetDelegateNominatableSupervisorRolesForSelfAssessment(int selfAssessmentId)
        {
            return connection.Query<SelfAssessmentSupervisorRole>(
               $@"SELECT ID, SelfAssessmentID, RoleName, RoleDescription, SelfAssessmentReview, ResultsReview
                  FROM   SelfAssessmentSupervisorRoles
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (AllowDelegateNomination = 1)
                  ORDER BY RoleName", new { selfAssessmentId }
               );
        }
        public SelfAssessmentSupervisorRole? GetSupervisorRoleById(int id)
        {
            return connection.Query<SelfAssessmentSupervisorRole>(
               $@"SELECT ID, SelfAssessmentID, RoleName, SelfAssessmentReview, ResultsReview
                  FROM   SelfAssessmentSupervisorRoles
                  WHERE (ID = @id)", new { id }
               ).FirstOrDefault();
        }

        public int EnrolDelegateOnAssessment(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId, int centreId, bool isLoggedInUser)
        {
            if (delegateUserId == 0 | supervisorDelegateId == 0 | selfAssessmentId == 0)
            {
                logger.LogWarning(
                    $"Not enrolling delegate on self assessment as it failed server side validation. delegateUserId: {delegateUserId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                );
                return -3;
            }

            var existingCandidateAssessment = connection.Query<CandidateAssessment>(
                @"SELECT ID, RemovedDate, CompletedDate
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (DelegateUserId = @delegateUserId)",
                new { selfAssessmentId, delegateUserId }
            ).FirstOrDefault();

            if (existingCandidateAssessment != null && existingCandidateAssessment.RemovedDate == null)
            {
                logger.LogWarning(
                     $"Not enrolling delegate on self assessment as they are already enrolled. delegateUserId: {delegateUserId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                 );
                return -2;
            }

            if (existingCandidateAssessment != null && existingCandidateAssessment.RemovedDate != null)
            {
                var existingCandidateAssessmentId = existingCandidateAssessment.Id;
                var numberOfAffectedRows = connection.Execute(
                    @"UPDATE CandidateAssessments
                            SET DelegateUserID = @delegateUserId,
                                SelfAssessmentID = @selfAssessmentId,
                                CompleteByDate = NULL,
                                EnrolmentMethodId = 2,
                                EnrolledByAdminId = @adminId,
                                CentreID = @centreId,
                                RemovedDate = NULL,
                                NonReportable = CASE WHEN NonReportable = 1 THEN NonReportable ELSE @isLoggedInUser END
                            WHERE ID = @existingCandidateAssessmentId",
                    new { delegateUserId, selfAssessmentId, adminId, centreId, existingCandidateAssessmentId, isLoggedInUser });

                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not enrolling delegate on self assessment as db update failed. delegateUserId: {delegateUserId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                    );
                    return -1;
                }
                var existingId = InsertCandidateAssessmentSupervisor(delegateUserId, supervisorDelegateId, selfAssessmentId, selfAssessmentSupervisorRoleId);
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO CandidateAssessments (DelegateUserID, SelfAssessmentID, CompleteByDate, EnrolmentMethodId, EnrolledByAdminId, CentreID,NonReportable)
                    VALUES (@delegateUserId, @selfAssessmentId, @completeByDate, 2, @adminId, @centreId,@isLoggedInUser)",
                    new { delegateUserId, selfAssessmentId, completeByDate, adminId, centreId, isLoggedInUser });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not enrolling delegate on self assessment as db insert failed. delegateUserId: {delegateUserId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                    );
                    return -1;
                }
                var existingId = InsertCandidateAssessmentSupervisor(delegateUserId, supervisorDelegateId, selfAssessmentId, selfAssessmentSupervisorRoleId);
                return existingId;
            }
        }
        public int InsertCandidateAssessmentSupervisor(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId)
        {
            int candidateAssessmentId = Convert.ToInt32(connection.ExecuteScalar(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                   WHERE (SelfAssessmentID = @selfAssessmentId) AND (DelegateUserID = @delegateUserId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS CandidateAssessmentID",
              new { selfAssessmentId, delegateUserId }));
            if (candidateAssessmentId > 0)
            {
                var candidateAssessmentSupervisorsId = Convert.ToInt32(connection.ExecuteScalar(
                    @"
                    SELECT COALESCE
                    ((SELECT ID
                        FROM CandidateAssessmentSupervisors
                        WHERE (CandidateAssessmentID = @candidateAssessmentId)
                            AND (SupervisorDelegateId = @supervisorDelegateId)                        
							AND ((SelfAssessmentSupervisorRoleID IS NULL) OR (SelfAssessmentSupervisorRoleID = @selfAssessmentSupervisorRoleId))), 0) AS CandidateAssessmentSupervisorID", new
                    { candidateAssessmentId, supervisorDelegateId, selfAssessmentSupervisorRoleId }));

                if (candidateAssessmentSupervisorsId == 0)
                {
                    //For a candidate assessment, only one supervisor role should be active (Removed = null)
                    var numberOfAffectedRows = connection.Execute(
                        @"BEGIN TRY
                            BEGIN TRANSACTION
                                UPDATE CandidateAssessmentSupervisors SET Removed = getUTCDate() WHERE CandidateAssessmentID = @candidateAssessmentId
                                        AND SupervisorDelegateId = @supervisorDelegateId
                                        AND Removed IS NULL

                                INSERT INTO CandidateAssessmentSupervisors (CandidateAssessmentID, SupervisorDelegateId, SelfAssessmentSupervisorRoleID)
                                        VALUES (@candidateAssessmentId, @supervisorDelegateId, @selfAssessmentSupervisorRoleId)

                            COMMIT TRANSACTION
                        END TRY
                        BEGIN CATCH
                            ROLLBACK TRANSACTION
                        END CATCH",
                        new { candidateAssessmentId, supervisorDelegateId, selfAssessmentSupervisorRoleId }
                    );
                }
                else
                {
                    //For a candidate assessment, only one supervisor role should be active (Removed = null)
                    int numberOfAffectedRows = connection.Execute(
                        @"BEGIN TRY
                            BEGIN TRANSACTION
                                UPDATE CandidateAssessmentSupervisors SET Removed = getUTCDate() WHERE CandidateAssessmentID = @candidateAssessmentId
                                        AND SupervisorDelegateId = @supervisorDelegateId
                                        AND Removed IS NULL

                                UPDATE CandidateAssessmentSupervisors SET Removed = NULL WHERE CandidateAssessmentID = @candidateAssessmentId
                                        AND SupervisorDelegateId = @supervisorDelegateId
                                        AND SelfAssessmentSupervisorRoleId = @selfAssessmentSupervisorRoleId

                            COMMIT TRANSACTION
                        END TRY
                        BEGIN CATCH
                            ROLLBACK TRANSACTION
                        END CATCH",
                        new { candidateAssessmentId, supervisorDelegateId, selfAssessmentSupervisorRoleId });
                }
            }
            return candidateAssessmentId;
        }
        public bool RemoveCandidateAssessment(int candidateAssessmentId)
        {
            var numberOfAffectedRows = connection.Execute(
         @"
                BEGIN TRY
                    BEGIN TRANSACTION
                        UPDATE CandidateAssessments SET RemovedDate = getUTCDate(), RemovalMethodID = 2
                            WHERE ID = @candidateAssessmentId AND RemovedDate IS NULL

                        COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH",
        new { candidateAssessmentId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not removing Candidate Assessment as db update failed. candidateAssessmentId: {candidateAssessmentId}"
                );
                return false;
            }
            return true;
        }
        public bool RemoveDelegateSelfAssessmentsupervisor(int candidateAssessmentId, int supervisorDelegateId)
        {
            var numberOfAffectedRows = connection.Execute(
         @"
                BEGIN TRY
                    BEGIN TRANSACTION
                         UPDATE CandidateAssessmentSupervisors SET Removed = getUTCDate()
                            WHERE (CandidateAssessmentID = @candidateAssessmentId) AND (SupervisorDelegateId=@supervisorDelegateId) AND (Removed IS NULL)

                        COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH",
        new { candidateAssessmentId, supervisorDelegateId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not removing Candidate Assessment Supervisors as db update failed. candidateAssessmentId: {candidateAssessmentId} " + $"supervisorDelegateId: {supervisorDelegateId}"
                );
                return false;
            }
            return true;
        }
        public bool RemoveCandidateAssessmentSupervisor(int selfAssessmentId, int supervisorDelegateId)
        {
            connection.Execute(
                @"DELETE FROM sarsv FROM SelfAssessmentResultSupervisorVerifications as sarsv
                    LEFT JOIN CandidateAssessmentSupervisors AS cas ON cas.ID = sarsv.CandidateAssessmentSupervisorID
                    INNER JOIN SelfAssessmentResults AS srs ON sarsv.SelfAssessmentResultId = srs.ID
                    INNER JOIN SelfAssessments AS sa ON srs.SelfAssessmentID = sa.ID
                    WHERE cas.SupervisorDelegateId = @supervisorDelegateId
                          AND cas.Removed IS NULL AND sarsv.Verified IS NULL
                          AND sa.ID = @selfAssessmentId", new { supervisorDelegateId, selfAssessmentId }
                );

            var deletedCandidateAssessmentSupervisors = connection.Execute(
                @"DELETE FROM cas
	                FROM CandidateAssessmentSupervisors AS cas
                    INNER JOIN CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID
				    LEFT JOIN CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
				    LEFT JOIN SelfAssessmentResultSupervisorVerifications AS sarsr ON cas.ID = sarsr.CandidateAssessmentSupervisorID
                    WHERE (ca.SelfAssessmentID = @selfAssessmentId) AND (cas.SupervisorDelegateId = @supervisorDelegateId)
		                AND (cas.Removed IS NULL)
					    AND (casv.ID IS NULL)
					    AND (sarsr.ID IS NULL)",
                new { selfAssessmentId, supervisorDelegateId });
            if (deletedCandidateAssessmentSupervisors < 1)
            {
                deletedCandidateAssessmentSupervisors = connection.Execute(
                    @"UPDATE cas SET Removed = getUTCDate()
                        FROM CandidateAssessmentSupervisors AS cas
	                    INNER JOIN CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID
                        WHERE (ca.SelfAssessmentID = @selfAssessmentId) AND (cas.SupervisorDelegateId = @supervisorDelegateId)",
                    new { selfAssessmentId, supervisorDelegateId });
            }

            if (deletedCandidateAssessmentSupervisors >= 1)
            {
                connection.Execute(
                    @"UPDATE SupervisorDelegates SET Removed = getUTCDate() 
                    WHERE ID = @supervisorDelegateId AND NOT EXISTS(
                        SELECT *
                        FROM CandidateAssessmentSupervisors
                        WHERE (SupervisorDelegateId = @supervisorDelegateId) AND (Removed IS NULL))",
                    new { supervisorDelegateId });
            }

            if (deletedCandidateAssessmentSupervisors < 1)
            {
                logger.LogWarning(
                    $"Not removing Candidate Assessment Supervisor as db update failed. selfAssessmentId: {selfAssessmentId}, supervisorDelegateId: {supervisorDelegateId}"
                );
                return false;
            }

            return true;
        }

        public void UpdateNotificationSent(int supervisorDelegateId)
        {
            connection.Execute(
                @"UPDATE SupervisorDelegates SET NotificationSent = getUTCDate() 
            WHERE ID = @supervisorDelegateId",
                new { supervisorDelegateId });
        }

        public bool InsertSelfAssessmentResultSupervisorVerification(int candidateAssessmentSupervisorId, int resultId)
        {
            //Set any existing verification requests to superceded:
            connection.Execute(@"UPDATE SelfAssessmentResultSupervisorVerifications SET Superceded = 1 WHERE SelfAssessmentResultId = @resultId", new { candidateAssessmentSupervisorId, resultId });
            //Insert a new SelfAssessmentResultSupervisorVerifications record:
            var numberOfAffectedRows = connection.Execute(
                     @"INSERT INTO SelfAssessmentResultSupervisorVerifications (CandidateAssessmentSupervisorID, SelfAssessmentResultId, EmailSent) VALUES (@candidateAssessmentSupervisorId, @resultId, GETUTCDATE())", new { candidateAssessmentSupervisorId, resultId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not inserting Self Assessment Result Supervisor Verification as db update failed. candidateAssessmentSupervisorId: {candidateAssessmentSupervisorId}, resultId: {resultId}"
                );
                return false;
            }
            return true;
        }

        public CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisorById(int candidateAssessmentSupervisorId)
        {
            return connection.Query<CandidateAssessmentSupervisor>(
               @"SELECT *
                  FROM   CandidateAssessmentSupervisors
                  WHERE (ID = @candidateAssessmentSupervisorId)", new { candidateAssessmentSupervisorId }
               ).FirstOrDefault();
        }

        public CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisor(int candidateAssessmentID, int supervisorDelegateId, int selfAssessmentSupervisorRoleId)
        {
            return connection.Query<CandidateAssessmentSupervisor>(
               @"SELECT *
                  FROM   CandidateAssessmentSupervisors
                  WHERE (CandidateAssessmentID = @candidateAssessmentID 
                        AND SupervisorDelegateId = @supervisorDelegateId 
                        AND SelfAssessmentSupervisorRoleId = @selfAssessmentSupervisorRoleId)",
               new { candidateAssessmentID, supervisorDelegateId, selfAssessmentSupervisorRoleId }
               ).FirstOrDefault();
        }

        public SelfAssessmentResultSummary? GetSelfAssessmentResultSummary(int candidateAssessmentId, int supervisorDelegateId)
        {
            return connection.Query<SelfAssessmentResultSummary>(
                @"SELECT ca.ID, ca.SelfAssessmentID, sa.Name AS RoleName, sa.ReviewerCommentsLabel, COALESCE (sasr.SelfAssessmentReview, 1) AS SelfAssessmentReview, COALESCE (sasr.ResultsReview, 1) AS SupervisorResultsReview, COALESCE (sasr.RoleName, 'Supervisor') AS SupervisorRoleTitle, ca.StartedDate, 
             ca.LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, npg.ProfessionalGroup, nsg.SubGroup, nr.RoleProfile, casv.ID AS CandidateAssessmentSupervisorVerificationId,
                 (SELECT COUNT(sas1.CompetencyID) AS CompetencyAssessmentQuestionCount
                 FROM    SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) OR
                              (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1)) AS CompetencyAssessmentQuestionCount,
                 (SELECT COUNT(sas1.CompetencyID) AS ResultCount
FROM   SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID LEFT OUTER JOIN
             SelfAssessmentResults AS sar1 ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
                 LEFT OUTER JOIN    CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) OR
             (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (caoc1.IncludedInSelfAssessment = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS ResultCount,
                 (SELECT COUNT(sas1.CompetencyID) AS VerifiedCount
FROM   SelfAssessmentResultSupervisorVerifications AS sasrv INNER JOIN
             SelfAssessmentResults AS sar1 ON sasrv.SelfAssessmentResultId = sar1.ID AND sasrv.Superceded = 0 RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
				 LEFT OUTER JOIN    CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1)) AS VerifiedCount,
                 (SELECT COUNT(sas1.CompetencyID) AS UngradedCount
FROM   SelfAssessmentResultSupervisorVerifications AS sasrv INNER JOIN
             SelfAssessmentResults AS sar1 ON sasrv.SelfAssessmentResultId = sar1.ID AND sasrv.Superceded = 0 LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
			 LEFT OUTER JOIN   CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.ID IS NULL)) AS UngradedCount,
                 (SELECT COUNT(sas1.CompetencyID) AS NotMeetingCount
FROM   SelfAssessmentResultSupervisorVerifications AS sasrv INNER JOIN
             SelfAssessmentResults AS sar1 ON sasrv.SelfAssessmentResultId = sar1.ID AND sasrv.Superceded = 0 LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
			 LEFT OUTER JOIN   CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 1)) AS NotMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS PartiallyMeeting
FROM   SelfAssessmentResultSupervisorVerifications AS sasrv INNER JOIN
             SelfAssessmentResults AS sar1 ON sasrv.SelfAssessmentResultId = sar1.ID AND sasrv.Superceded = 0 LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
			 LEFT OUTER JOIN   CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 2)) AS PartiallyMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS MeetingCount
FROM   SelfAssessmentResultSupervisorVerifications AS sasrv INNER JOIN
             SelfAssessmentResults AS sar1 ON sasrv.SelfAssessmentResultId = sar1.ID AND sasrv.Superceded = 0 LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
			 LEFT OUTER JOIN   CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (sasrv.SignedOff = 1) AND (caqrr1.LevelRAG = 3)) AS MeetingCount,
              sa.SignOffSupervisorStatement,ca.DelegateUserID
FROM   NRPProfessionalGroups AS npg RIGHT OUTER JOIN
             NRPSubGroups AS nsg RIGHT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr RIGHT OUTER JOIN
             SelfAssessments AS sa INNER JOIN
             CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID AND casv.Verified IS NULL AND cas.Removed IS NULL INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID ON sa.ID = ca.SelfAssessmentID ON sasr.ID = cas.SelfAssessmentSupervisorRoleID ON nsg.ID = sa.NRPSubGroupID ON npg.ID = sa.NRPProfessionalGroupID LEFT OUTER JOIN
             NRPRoles AS nr ON sa.NRPRoleID = nr.ID
WHERE (cas.CandidateAssessmentID = @candidateAssessmentId) AND (cas.SupervisorDelegateId = @supervisorDelegateId)", new { candidateAssessmentId, supervisorDelegateId }
                ).FirstOrDefault();
        }

        public void UpdateCandidateAssessmentSupervisorVerificationById(int? candidateAssessmentSupervisorVerificationId, string? supervisorComments, bool signedOff)
        {
            connection.Execute(
        @"UPDATE CandidateAssessmentSupervisorVerifications SET Verified = getUTCDate(), Comments = @supervisorComments, SignedOff = @signedOff
            WHERE ID = @candidateAssessmentSupervisorVerificationId",
       new { candidateAssessmentSupervisorVerificationId, supervisorComments, signedOff });
        }
        public IEnumerable<CandidateAssessmentSupervisorVerificationSummary> GetCandidateAssessmentSupervisorVerificationSummaries(int candidateAssessmentId)
        {
            return connection.Query<CandidateAssessmentSupervisorVerificationSummary>(
                @"SELECT	ca1.ID, 
		                   u.FirstName AS Forename, 
                           u.LastName AS Surname, 
                           u.PrimaryEmail AS Email, 
                            COUNT(sas1.CompetencyID) AS VerifiedCount, ac.Active AS AdminActive
                    FROM   SelfAssessmentResultSupervisorVerifications AS sasrv
                    INNER JOIN SelfAssessmentResults AS sar1 
	                    ON sasrv.SelfAssessmentResultId = sar1.ID  AND sasrv.Superceded = 0
                    INNER JOIN CandidateAssessmentSupervisors 
	                    ON sasrv.CandidateAssessmentSupervisorID = CandidateAssessmentSupervisors.ID 
                    INNER JOIN SupervisorDelegates sd
	                    ON CandidateAssessmentSupervisors.SupervisorDelegateId = sd.ID 
                    INNER JOIN AdminAccounts AS ac
                            ON sd.SupervisorAdminID = ac.ID
                              INNER JOIN Users AS u
                                 ON ac.UserID = u.ID
                    RIGHT OUTER JOIN SelfAssessmentStructure AS sas1 
                    INNER JOIN CandidateAssessments AS ca1 
	                    ON sas1.SelfAssessmentID = ca1.SelfAssessmentID 
                    INNER JOIN CompetencyAssessmentQuestions AS caq1 
	                    ON sas1.CompetencyID = caq1.CompetencyID 
	                    ON sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID
                    LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS caoc1 
	                    ON sas1.CompetencyID = caoc1.CompetencyID 
	                    AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID 
	                    AND ca1.ID = caoc1.CandidateAssessmentID
                    WHERE (ca1.ID = @candidateAssessmentId) 
	                    AND (sas1.Optional = 0) 
	                    AND (NOT (sar1.Result IS NULL)) 
	                    AND (sasrv.SignedOff = 1) 
	                    OR (ca1.ID = @candidateAssessmentId) 
	                    AND (caoc1.IncludedInSelfAssessment = 1) 
	                    AND (NOT (sar1.Result IS NULL)) 
	                    AND (sasrv.SignedOff = 1) 
	                    OR (ca1.ID = @candidateAssessmentId) 
	                    AND (sas1.Optional = 0) 
	                    AND (NOT (sar1.SupportingComments IS NULL)) 
	                    AND (sasrv.SignedOff = 1) 
	                    OR (ca1.ID = @candidateAssessmentId)
	                    AND (caoc1.IncludedInSelfAssessment = 1) 
	                    AND (NOT (sar1.SupportingComments IS NULL)) 
	                    AND (sasrv.SignedOff = 1)
                      GROUP BY u.FirstName, u.LastName, u.PrimaryEmail, caoc1.CandidateAssessmentID, ca1.ID, ac.Active
                    ORDER BY u.LastName, u.FirstName", new { candidateAssessmentId });
        }

        public int IsSupervisorDelegateExistAndReturnId(int? supervisorAdminId, string delegateEmail, int centreId)
        {
            int? delegateUserId = (int?)connection.ExecuteScalar(
                 @"SELECT da.UserID AS DelegateUserID 
                            FROM Users u
                            INNER JOIN DelegateAccounts da
                            ON da.UserID = u.ID
	                        LEFT JOIN UserCentreDetails ucd
	                        ON ucd.UserID = u.ID
                            AND ucd.CentreID = da.CentreID
                            WHERE (ucd.Email = @delegateEmail OR u.PrimaryEmail = @delegateEmail)
                            AND u.Active = 1 
                            AND da.CentreID = @centreId", new { delegateEmail, centreId });

            int? existingId = (int?)connection.ExecuteScalar(
                @"
                   SELECT ID
                        FROM    SupervisorDelegates sd
                        WHERE (sd.SupervisorAdminID = @supervisorAdminID OR @supervisorAdminID = 0) AND (sd.DelegateUserID = @delegateUserId OR @delegateUserID = 0) AND DelegateEmail = @delegateEmail
                        ORDER BY ID DESC
                        ",
                new
                {
                    supervisorAdminId = supervisorAdminId ?? 0,
                    delegateUserId = delegateUserId ?? 0,
                    delegateEmail
                }
            );

            return existingId ?? 0;
        }

        public SupervisorDelegate GetSupervisorDelegateById(int supervisorDelegateId)
        {
            var supervisorDelegate = connection.Query<SupervisorDelegate>(
                $@"SELECT ID, SupervisorAdminID, DelegateEmail, Added
                            ,NotificationSent, Removed, SupervisorEmail, AddedByDelegate
                            ,InviteHash, DelegateUserID
                    FROM SupervisorDelegates
                    WHERE ID = @supervisorDelegateId", new { supervisorDelegateId }
            ).FirstOrDefault();

            return supervisorDelegate!;
        }

        public void RemoveCandidateAssessmentSupervisorVerification(int id)
        {
            connection.Execute(
                @"DELETE 
	                FROM CandidateAssessmentSupervisorVerifications WHERE
                    ID = @id ", new { id });
        }

        public void UpdateCandidateAssessmentNonReportable(int candidateAssessmentId)
        {
            connection.Execute(
                @"UPDATE CandidateAssessments
                    SET NonReportable = 1
                    FROM CandidateAssessments AS CA
                    INNER JOIN CandidateAssessmentSupervisors AS CAS ON CA.ID = cas.CandidateAssessmentID AND CAS.Removed IS NULL
                    INNER JOIN SupervisorDelegates AS SD ON SD.ID = CAS.SupervisorDelegateId
                    INNER JOIN AdminAccounts AS AA ON AA.ID = SD.SupervisorAdminID AND AA.UserID = SD.DelegateUserID
                        WHERE CA.ID = @candidateAssessmentId AND NonReportable = 0 ", new { candidateAssessmentId });
        }
    }
}
