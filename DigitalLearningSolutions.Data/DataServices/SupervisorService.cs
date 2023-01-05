namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using Microsoft.Extensions.Logging;

    public interface ISupervisorService
    {
        //GET DATA
        DashboardData GetDashboardDataForAdminId(int adminId);
        IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId);
        SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId, int adminId, int delegateId);
        IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int adminId);
        DelegateSelfAssessment GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedSignOffs(int adminId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedReviews(int adminId);
        DelegateSelfAssessment GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId);
        IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int candidateId, int centreId);
        RoleProfile GetRoleProfileById(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesForSelfAssessment(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetDelegateNominatableSupervisorRolesForSelfAssessment(int selfAssessmentId);
        SelfAssessmentSupervisorRole GetSupervisorRoleById(int id);
        DelegateSelfAssessment GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(int selfAssessmentId, int supervisorDelegateId);
        DelegateSelfAssessment GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(int candidateAssessmentId, int supervisorDelegateId);
        CandidateAssessmentSupervisor GetCandidateAssessmentSupervisorById(int candidateAssessmentSupervisorId);
        SelfAssessmentResultSummary GetSelfAssessmentResultSummary(int candidateAssessmentId, int supervisorDelegateId);
        IEnumerable<CandidateAssessmentSupervisorVerificationSummary> GetCandidateAssessmentSupervisorVerificationSummaries(int candidateAssessmentId);
        IEnumerable<SupervisorForEnrolDelegate> GetSupervisorForEnrolDelegate(int CustomisationID, int CentreID);
        //UPDATE DATA
        bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool RemoveSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerifications(int selfAssessmentResultSupervisorVerificationId, string? comments, bool signedOff, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(int selfAssessmentResultSupervisorVerificationId);
        int RemoveSelfAssessmentResultSupervisorVerificationById(int id);
        bool RemoveCandidateAssessment(int candidateAssessmentId);
        void UpdateNotificationSent(int supervisorDelegateId);
        void UpdateCandidateAssessmentSupervisorVerificationById(int? candidateAssessmentSupervisorVerificationId, string? supervisorComments, bool signedOff);
        //INSERT DATA
        int AddSuperviseDelegate(int? supervisorAdminId, int? delegateId, string delegateEmail, string supervisorEmail, int centreId);
        int EnrolDelegateOnAssessment(int delegateId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId);
        int InsertCandidateAssessmentSupervisor(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId);
        bool InsertSelfAssessmentResultSupervisorVerification(int candidateAssessmentSupervisorId, int resultId);
        //DELETE DATA
        bool RemoveCandidateAssessmentSupervisor(int selfAssessmentId, int supervisorDelegateId);
    }
    public class SupervisorService : ISupervisorService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorService> logger;
        private const string supervisorDelegateDetailFields = @"sd.ID, sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateUserID,
            sd.DelegateEmail, sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Removed,
            sd.InviteHash, u.FirstName, u.LastName, jg.JobGroupName, da.Answer1, da.Answer2, da.Answer3, da.Answer4, da.Answer5,
            da.Answer6, da.CandidateNumber, u.ProfessionalRegistrationNumber, u.PrimaryEmail AS CandidateEmail, cp1.CustomPrompt AS CustomPrompt1, cp2.CustomPrompt AS CustomPrompt2,
            cp3.CustomPrompt AS CustomPrompt3, cp4.CustomPrompt AS CustomPrompt4, cp5.CustomPrompt AS CustomPrompt5,
            cp6.CustomPrompt AS CustomPrompt6, COALESCE(au.CentreID, da.CentreID) AS CentreID,
            au.Forename + ' ' + au.Surname AS SupervisorName, (SELECT COUNT(cas.ID)
            FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
            CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID
            WHERE (cas.SupervisorDelegateId = sd.ID) AND (ca.RemovedDate IS NULL)) AS CandidateAssessmentCount,
            CAST(COALESCE (au2.NominatedSupervisor, 0) AS Bit) AS DelegateIsNominatedSupervisor, CAST(COALESCE (au2.Supervisor, 0) AS Bit) AS DelegateIsSupervisor ";
        private const string supervisorDelegateDetailTables = @"SupervisorDelegates AS sd 
            INNER JOIN Users u
	            ON u.id = sd.DelegateUserID
            FULL OUTER JOIN JobGroups AS jg 		
	            ON jg.JobGroupID = u.JobGroupID
            INNER JOIN dbo.DelegateAccounts da 
	            ON u.ID = da.UserID
            LEFT OUTER JOIN AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID 
            FULL OUTER JOIN CustomPrompts AS cp6 
            RIGHT OUTER JOIN CustomPrompts AS cp1 
            RIGHT OUTER JOIN Centres AS ct ON cp1.CustomPromptID = ct.CustomField1PromptID 
            LEFT OUTER JOIN CustomPrompts AS cp2 ON ct.CustomField2PromptID = cp2.CustomPromptID 
            LEFT OUTER JOIN CustomPrompts AS cp3 ON ct.CustomField3PromptID = cp3.CustomPromptID 
            LEFT OUTER JOIN CustomPrompts AS cp4 ON ct.CustomField4PromptID = cp4.CustomPromptID 
            LEFT OUTER JOIN CustomPrompts AS cp5 ON ct.CustomField5PromptID = cp5.CustomPromptID ON cp6.CustomPromptID = ct.CustomField6PromptID 
            ON ct.CentreID = da.CentreID		
            FULL OUTER JOIN AdminUsers AS au2 
	            ON au2.CentreID = da.CentreID 
	            AND au2.Email = u.PrimaryEmail 
	            AND au2.Active = 1 
	            AND au2.Approved = 1 
	            AND au2.Email IS NOT NULL ";
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

        public SupervisorService(IDbConnection connection, ILogger<SupervisorService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public DashboardData GetDashboardDataForAdminId(int adminId)
        {
            return connection.Query<DashboardData>(
                @"SELECT (SELECT COUNT(SupervisorDelegates.ID) AS StaffCount
                    FROM    SupervisorDelegates LEFT OUTER JOIN
                            Candidates ON SupervisorDelegates.CandidateID = Candidates.CandidateID AND Candidates.Active = 1
                     WHERE  (SupervisorDelegates.SupervisorAdminID = @adminId) AND (SupervisorDelegates.Removed IS NULL)) AS StaffCount,
                         (SELECT COUNT(ID) AS StaffCount
                         FROM    SupervisorDelegates AS SupervisorDelegates_1
                         WHERE (SupervisorAdminID = @adminId) AND (CandidateID IS NULL) AND (Removed IS NULL)) AS StaffUnregisteredCount,
                    (SELECT COUNT(ca.ID) AS ProfileSelfAssessmentCount
                         FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                           CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                             SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID
                    WHERE (sd.SupervisorAdminID = @adminId) AND ((ca.RemovedDate IS NULL))) AS ProfileSelfAssessmentCount,
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
                    WHERE (sd.SupervisorAdminID = @adminId) AND ((ca.RemovedDate IS NULL)) AND (casv.Verified IS NULL)
                    ), 0) AS AwaitingReviewCount", new { adminId }
                ).FirstOrDefault();
        }

        public IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId)
        {
            return connection.Query<SupervisorDelegateDetail>(
                $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.SupervisorAdminID = @adminId) AND (Removed IS NULL)
                    ORDER BY u.LastName, COALESCE(u.FirstName, sd.DelegateEmail)", new { adminId }
                );
        }
        public int AddSuperviseDelegate(int? supervisorAdminId, int? delegateId, string delegateEmail, string supervisorEmail, int centreId)
        {
            var addedByDelegate = (delegateId != null);
            if (delegateEmail.Length == 0 | supervisorEmail.Length == 0)
            {
                logger.LogWarning(
                    $"Not adding delegate to SupervisorDelegates as it failed server side validation. supervisorAdminId: {supervisorAdminId}, delegateEmail: {delegateEmail}"
                );
                return -3;
            }
            int existingId = (int)connection.ExecuteScalar(
               @"SELECT COALESCE
                 ((SELECT ID
                  FROM    SupervisorDelegates
                  WHERE (SupervisorEmail = @supervisorEmail) AND (DelegateEmail = @delegateEmail)), 0) AS ID",
               new { supervisorEmail, delegateEmail });
            if (existingId > 0)
            {
                var numberOfAffectedRows = connection.Execute(@"UPDATE SupervisorDelegates SET Removed = NULL WHERE (SupervisorAdminID = @supervisorAdminId) AND (DelegateEmail = @delegateEmail) AND (Removed IS NOT NULL)", new { supervisorAdminId, delegateEmail });
                return existingId;
            }
            else
            {
                if (delegateId == null)
                {
                    delegateId = (int?)connection.ExecuteScalar(
                        @"SELECT CandidateID FROM Candidates WHERE EmailAddress = @delegateEmail AND Active = 1 AND CentreID = @centreId", new { delegateEmail, centreId }
                        );
                }
                if (supervisorAdminId == null)
                {
                    supervisorAdminId = (int?)connection.ExecuteScalar(
                    @"SELECT AdminID FROM AdminUsers WHERE Email = @supervisorEmail AND Active = 1 AND CentreID = @centreId", new { supervisorEmail, centreId }
                    );
                }
                if (supervisorAdminId != null)
                {
                    connection.Execute(@"UPDATE AdminUsers SET Supervisor = 1 WHERE AdminID = @supervisorAdminId AND Supervisor = 0", new { supervisorAdminId });
                }
                var numberOfAffectedRows = connection.Execute(
         @"INSERT INTO SupervisorDelegates (SupervisorAdminID, DelegateEmail, CandidateID, SupervisorEmail, AddedByDelegate)
                    VALUES (@supervisorAdminId, @delegateEmail, @delegateId, @supervisorEmail, @addedByDelegate)",
        new { supervisorAdminId, delegateEmail, delegateId, supervisorEmail, addedByDelegate });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting SupervisorDelegate as db insert failed. supervisorAdminId: {supervisorAdminId}, delegateEmail: {delegateEmail}, delegateId: {delegateId}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE
                 ((SELECT ID
                  FROM    SupervisorDelegates
                  WHERE (SupervisorEmail = @supervisorEmail) AND (DelegateEmail = @delegateEmail)), 0) AS AdminID",
               new { supervisorEmail, delegateEmail });
                return existingId;
            }
        }

        public SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId, int adminId, int delegateId)
        {
            return connection.Query<SupervisorDelegateDetail>(
               $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.ID = @supervisorDelegateId) AND (sd.CandidateID = @delegateId OR sd.SupervisorAdminID = @adminId) AND (Removed IS NULL)", new { supervisorDelegateId, adminId, delegateId }
               ).FirstOrDefault();
        }

        public IEnumerable<SupervisorForEnrolDelegate> GetSupervisorForEnrolDelegate(int CustomisationID, int CentreID)
        {
            return connection.Query<SupervisorForEnrolDelegate>(
                $@"SELECT AdminID, Forename + ' ' + Surname AS Name, Email FROM AdminUsers AS au
                    WHERE (Supervisor = 1) AND (CentreID = @CentreID) AND (CategoryID = 0 OR
                         CategoryID = (SELECT au.CategoryID FROM Applications AS a INNER JOIN
                           Customisations AS c ON a.ApplicationID = c.ApplicationID WHERE        (c.CustomisationID = @CustomisationID))) AND (Active = 1) AND (Approved = 1) GROUP BY AdminID, Surname, Forename, Email ORDER BY Surname, Forename",
                new { CentreID, CustomisationID });
        }

        public bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId)
        {
            var numberOfAffectedRows = connection.Execute(
         @"UPDATE SupervisorDelegates SET Confirmed = getUTCDate()
            WHERE ID = @supervisorDelegateId AND Confirmed IS NULL AND Removed IS NULL AND (CandidateID = @candidateId OR SupervisorAdminID = @adminId)",
        new { supervisorDelegateId, candidateId, adminId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not confirming SupervisorDelegate as db update failed. supervisorDelegateId: {supervisorDelegateId}, candidateId: {candidateId}, adminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public bool RemoveSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId)
        {
            var numberOfAffectedRows = connection.Execute(
         @"UPDATE SupervisorDelegates SET Removed = getUTCDate()
            WHERE ID = @supervisorDelegateId AND Removed IS NULL AND (CandidateID = @candidateId OR SupervisorAdminID = @adminId)",
        new { supervisorDelegateId, candidateId, adminId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not removing SupervisorDelegate as db update failed. supervisorDelegateId: {supervisorDelegateId}, candidateId: {candidateId}, adminId: {adminId}"
                );
                return false;
            }
            return true;
        }

        public IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int adminId)
        {
            return connection.Query<DelegateSelfAssessment>(
                @$"SELECT {delegateSelfAssessmentFields}, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, r.RoleProfile, sg.SubGroup, pg.ProfessionalGroup,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL)) AS ResultsVerificationRequests
                 FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                 CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                 SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID LEFT OUTER JOIN
                 NRPProfessionalGroups AS pg ON sa.NRPProfessionalGroupID = pg.ID LEFT OUTER JOIN
                 NRPSubGroups AS sg ON sa.NRPSubGroupID = sg.ID LEFT OUTER JOIN
                 NRPRoles AS r ON sa.NRPRoleID = r.ID
                 LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                 WHERE (ca.RemovedDate IS NULL) AND (cas.SupervisorDelegateId = @supervisorDelegateId)", new { supervisorDelegateId }
                );
        }
        public DelegateSelfAssessment GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(int selfAssessmentId, int supervisorDelegateId)
        {
            return connection.Query<DelegateSelfAssessment>(
               @$"SELECT {delegateSelfAssessmentFields}, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
                    FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL)) AS ResultsVerificationRequests
                FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID
                 LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                WHERE  (ca.RemovedDate IS NULL) AND (cas.SupervisorDelegateId = @supervisorDelegateId) AND (sa.ID = @selfAssessmentId)", new { selfAssessmentId, supervisorDelegateId }
               ).FirstOrDefault();
        }
        public DelegateSelfAssessment GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId)
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
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL)) AS ResultsVerificationRequests
                FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID
                WHERE (ca.ID = @candidateAssessmentId)", new { candidateAssessmentId }
               ).FirstOrDefault();
        }
        public DelegateSelfAssessment GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(int candidateAssessmentId, int supervisorDelegateId)
        {
            return connection.Query<DelegateSelfAssessment>(
               @$"SELECT {delegateSelfAssessmentFields}, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
                {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
                    FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL)) AS ResultsVerificationRequests
                FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID
                 LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                WHERE  (ca.RemovedDate IS NULL) AND (cas.SupervisorDelegateId = @supervisorDelegateId) AND (ca.ID = @candidateAssessmentId)", new { candidateAssessmentId, supervisorDelegateId }
               ).FirstOrDefault();
        }
        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedSignOffs(int adminId)
        {
            return connection.Query<SupervisorDashboardToDoItem>(
                @"SELECT ca.ID, sd.ID AS SupervisorDelegateId, c.FirstName + ' ' + c.LastName AS DelegateName, sa.Name AS ProfileName, casv.Requested, 1 AS SignOffRequest, 0 AS ResultsReviewRequest
                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                    CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                    SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                    CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID INNER JOIN
                    Candidates AS c ON ca.CandidateID = c.CandidateID
                WHERE (sd.SupervisorAdminID = @adminId) AND (casv.Verified IS NULL) AND (sd.Removed IS NULL)", new { adminId }
                );
        }
        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedReviews(int adminId)
        {
            return connection.Query<SupervisorDashboardToDoItem>(
                @"SELECT ca.ID, sd.ID AS SupervisorDelegateId, c.FirstName + ' ' + c.LastName AS DelegateName, sa.Name AS ProfileName, MAX(sasv.Requested) AS Requested, 0 AS SignOffRequest, 1 AS ResultsReviewRequest
                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                    CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                    Candidates AS c ON ca.CandidateID = c.CandidateID INNER JOIN
                    SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
					SelfAssessmentResults AS sar ON sar.SelfAssessmentID = sa.ID INNER JOIN
					Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN					
                    SelfAssessmentResultSupervisorVerifications AS sasv ON sasv.SelfAssessmentResultId = sar.ID
                        AND sasv.CandidateAssessmentSupervisorID = cas.ID AND sar.DateTime = (
						    SELECT MAX(sar2.DateTime)
						    FROM SelfAssessmentResults AS sar2
						    WHERE sar2.SelfAssessmentID = sar.SelfAssessmentID AND sar2.CompetencyID = co.ID
					)
                WHERE (sd.SupervisorAdminID = @adminId) AND (sasv.Verified IS NULL) AND (sd.Removed IS NULL)
				GROUP BY sa.ID, ca.ID, sd.ID, c.FirstName, c.LastName, sa.Name", new { adminId }
                );
        }

        public DelegateSelfAssessment GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId)
        {
            return connection.Query<DelegateSelfAssessment>(
                @$"SELECT ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, sa.ReviewerCommentsLabel, COALESCE (sasr.RoleName, 'Supervisor') AS SupervisorRoleTitle, ca.StartedDate, ca.LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, r.RoleProfile, sg.SubGroup, pg.ProfessionalGroup, sa.SupervisorResultsReview AS IsSupervisorResultsReviewed,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS SignOffRequested,
               {signedOffFields}
                 (SELECT COUNT(*) AS Expr1
                    FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL)) AS ResultsVerificationRequests
                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                           SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID INNER JOIN
                             SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID LEFT OUTER JOIN
                             NRPProfessionalGroups AS pg ON sa.NRPProfessionalGroupID = pg.ID LEFT OUTER JOIN
                             NRPSubGroups AS sg ON sa.NRPSubGroupID = sg.ID LEFT OUTER JOIN
                             NRPRoles AS r ON sa.NRPRoleID = r.ID
                             LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                WHERE (ca.ID = @candidateAssessmentId) AND (sd.SupervisorAdminID = @adminId)",
                new { candidateAssessmentId, adminId }
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
        public IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int delegateUserId, int centreId)
        {
            return connection.Query<RoleProfile>(
                $@"SELECT rp.ID, rp.Name AS RoleProfileName, rp.Description, rp.BrandID, rp.ParentSelfAssessmentID, rp.[National], rp.[Public], rp.CreatedByAdminID AS OwnerAdminID, rp.NRPProfessionalGroupID, rp.NRPSubGroupID, rp.NRPRoleID, rp.PublishStatusID, 0 AS UserRole, rp.CreatedDate,
                 (SELECT BrandName
                 FROM    Brands
                 WHERE (BrandID = rp.BrandID)) AS Brand,
                 '' AS ParentSelfAssessment,
                 '' AS Owner, rp.Archived, rp.LastEdit,
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
                 WHERE (DelegateUserID = @delegateUserId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)))", new { delegateUserId, centreId }
                );
        }

        public RoleProfile GetRoleProfileById(int selfAssessmentId)
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
               $@"SELECT ID, SelfAssessmentID, RoleName, RoleDescription, SelfAssessmentReview, ResultsReview
                  FROM   SelfAssessmentSupervisorRoles
                  WHERE (SelfAssessmentID = @selfAssessmentId)
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
        public SelfAssessmentSupervisorRole GetSupervisorRoleById(int id)
        {
            return connection.Query<SelfAssessmentSupervisorRole>(
               $@"SELECT ID, SelfAssessmentID, RoleName, SelfAssessmentReview, ResultsReview
                  FROM   SelfAssessmentSupervisorRoles
                  WHERE (ID = @id)", new { id }
               ).FirstOrDefault();
        }

        public int EnrolDelegateOnAssessment(int delegateId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId)
        {
            if (delegateId == 0 | supervisorDelegateId == 0 | selfAssessmentId == 0)
            {
                logger.LogWarning(
                    $"Not enrolling delegate on self assessment as it failed server side valiidation. delegateId: {delegateId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                );
                return -3;
            }
            int existingId = (int)connection.ExecuteScalar(
               @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                  WHERE (SelfAssessmentID = @selfAssessmentId) AND (CandidateID = @delegateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS ID",
               new { selfAssessmentId, delegateId });
            if (existingId > 0)
            {
                logger.LogWarning(
                     $"Not enrolling delegate on self assessment as they are already enroled. delegateId: {delegateId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                 );
                return -2;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
         @"INSERT INTO CandidateAssessments (CandidateID, SelfAssessmentID, CompleteByDate, EnrolmentMethodId, EnrolledByAdminId)
                    VALUES (@delegateId, @selfAssessmentId, @completeByDate, 2, @adminId)",
        new { delegateId, selfAssessmentId, completeByDate, adminId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not enrolling delegate on self assessment as db insert failed. delegateId: {delegateId}, supervisorDelegateId: {supervisorDelegateId}, selfAssessmentId: {selfAssessmentId}"
                    );
                    return -1;
                }
                existingId = InsertCandidateAssessmentSupervisor(delegateId, supervisorDelegateId, selfAssessmentId, selfAssessmentSupervisorRoleId);
                return existingId;
            }
        }
        public int InsertCandidateAssessmentSupervisor(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId)
        {
            int candidateAssessmentId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                   WHERE (SelfAssessmentID = @selfAssessmentId) AND (DelegateUserID = @delegateUserId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS CandidateAssessmentID",
               new { selfAssessmentId, delegateUserId });
            if (candidateAssessmentId > 0)
            {
                int numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO CandidateAssessmentSupervisors (CandidateAssessmentID, SupervisorDelegateId, SelfAssessmentSupervisorRoleID)
                            VALUES (@candidateAssessmentId, @supervisorDelegateId, @selfAssessmentSupervisorRoleId)", new { candidateAssessmentId, supervisorDelegateId, selfAssessmentSupervisorRoleId }
                    );
            }
            return candidateAssessmentId;
        }
        public bool RemoveCandidateAssessment(int candidateAssessmentId)
        {
            var numberOfAffectedRows = connection.Execute(
         @"UPDATE CandidateAssessments SET RemovedDate = getUTCDate(), RemovalMethodID = 2
            WHERE ID = @candidateAssessmentId AND RemovedDate IS NULL",
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

        public bool RemoveCandidateAssessmentSupervisor(int selfAssessmentId, int supervisorDelegateId)
        {
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
            connection.Execute(@"UPDATE SelfAssessmentResultSupervisorVerifications SET Superceded = 1 WHERE CandidateAssessmentSupervisorID = @candidateAssessmentSupervisorId AND SelfAssessmentResultId = @resultId", new { candidateAssessmentSupervisorId, resultId });
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

        public CandidateAssessmentSupervisor GetCandidateAssessmentSupervisorById(int candidateAssessmentSupervisorId)
        {
            return connection.Query<CandidateAssessmentSupervisor>(
               @"SELECT *
                  FROM   CandidateAssessmentSupervisors
                  WHERE (ID = @candidateAssessmentSupervisorId)", new { candidateAssessmentSupervisorId }
               ).FirstOrDefault();
        }

        public SelfAssessmentResultSummary GetSelfAssessmentResultSummary(int candidateAssessmentId, int supervisorDelegateId)
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
             SelfAssessmentResults AS sar1 ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) OR
             (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (caoc1.IncludedInSelfAssessment = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS ResultCount,
                 (SELECT COUNT(sas1.CompetencyID) AS VerifiedCount
FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
             SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1)) AS VerifiedCount,
                 (SELECT COUNT(sas1.CompetencyID) AS UngradedCount
FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
             SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.ID IS NULL)) AS UngradedCount,
                 (SELECT COUNT(sas1.CompetencyID) AS NotMeetingCount
FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
             SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 1)) AS NotMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS PartiallyMeeting
FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
             SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 2)) AS PartiallyMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS MeetingCount
FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
             SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
             CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
             sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
             (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
             (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caqrr1.LevelRAG = 3)) AS MeetingCount,
              sa.SignOffSupervisorStatement
FROM   NRPProfessionalGroups AS npg RIGHT OUTER JOIN
             NRPSubGroups AS nsg RIGHT OUTER JOIN
             SelfAssessmentSupervisorRoles AS sasr RIGHT OUTER JOIN
             SelfAssessments AS sa INNER JOIN
             CandidateAssessmentSupervisorVerifications AS casv INNER JOIN
             CandidateAssessmentSupervisors AS cas ON casv.CandidateAssessmentSupervisorID = cas.ID AND casv.Verified IS NULL INNER JOIN
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
                @"SELECT ca1.ID, AdminUsers.Forename, AdminUsers.Surname, AdminUsers.Email, COUNT(sas1.CompetencyID) AS VerifiedCount
FROM   SelfAssessmentResultSupervisorVerifications INNER JOIN
             SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID INNER JOIN
             CandidateAssessmentSupervisors ON SelfAssessmentResultSupervisorVerifications.CandidateAssessmentSupervisorID = CandidateAssessmentSupervisors.ID INNER JOIN
             SupervisorDelegates ON CandidateAssessmentSupervisors.SupervisorDelegateId = SupervisorDelegates.ID INNER JOIN
             AdminUsers ON SupervisorDelegates.SupervisorAdminID = AdminUsers.AdminID RIGHT OUTER JOIN
             SelfAssessmentStructure AS sas1 INNER JOIN
             CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
             CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                 (SELECT MAX(ID) AS Expr1
                 FROM    SelfAssessmentResults AS sar2
                 WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
             CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
WHERE (ca1.ID = @candidateAssessmentId) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
             (ca1.ID = @candidateAssessmentId) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
             (ca1.ID = @candidateAssessmentId) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
             (ca1.ID = @candidateAssessmentId) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1)
GROUP BY AdminUsers.Forename, AdminUsers.Surname, AdminUsers.Email, caoc1.CandidateAssessmentID, ca1.ID
ORDER BY AdminUsers.Surname, AdminUsers.Forename", new { candidateAssessmentId });
        }
    }
}
