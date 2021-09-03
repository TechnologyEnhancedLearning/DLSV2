namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;
    using System;

    public interface ISupervisorService
    {
        //GET DATA
        DashboardData GetDashboardDataForAdminId(int adminId);
        IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId);
        SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId);
        IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int adminId);
        DelegateSelfAssessment GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItems(int adminId);
        DelegateSelfAssessment GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId);
        IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int candidateId, int centreId);
        RoleProfile GetRoleProfileById(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesForSelfAssessment(int selfAssessmentId);
        SelfAssessmentSupervisorRole GetSupervisorRoleById(int id);
        //UPDATE DATA
        bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool RemoveSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerifications(int selfAssessmentResultSupervisorVerificationId, string? comments, bool signedOff, int adminId);
        bool RemoveCandidateAssessment(int candidateAssessmentId);
        //INSERT DATA
        int AddSuperviseDelegate(int? supervisorAdminId, int? delegateId, string delegateEmail, string supervisorEmail, int centreId);
        int EnrolDelegateOnAssessment(int delegateId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId);
        int InsertCandidateAssessmentSupervisor(int delegateId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId);
        //DELETE DATA


    }
    public class SupervisorService : ISupervisorService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorService> logger;
        private const string supervisorDelegateDetailFields = @"sd.ID, sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateEmail, sd.CandidateID, sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Confirmed, sd.Removed, c.FirstName, c.LastName, jg.JobGroupName, c.Answer1, c.Answer2, c.Answer3, c.Answer4, c.Answer5, c.Answer6, c.CandidateNumber,
             cp1.CustomPrompt AS CustomPrompt1, cp2.CustomPrompt AS CustomPrompt2, cp3.CustomPrompt AS CustomPrompt3, cp4.CustomPrompt AS CustomPrompt4, cp5.CustomPrompt AS CustomPrompt5, cp6.CustomPrompt AS CustomPrompt6, COALESCE(au.CentreID, c.CentreID)
             AS CentreID, au.Forename + ' ' + au.Surname AS SupervisorName, (SELECT COUNT(cas.ID)
FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
             CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID
WHERE (cas.SupervisorDelegateId = sd.ID) AND (ca.RemovedDate IS NULL)) AS CandidateAssessmentCount ";
        private const string supervisorDelegateDetailTables = @"SupervisorDelegates AS sd LEFT OUTER JOIN
             AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID FULL OUTER JOIN
             CustomPrompts AS cp6 RIGHT OUTER JOIN
             CustomPrompts AS cp1 RIGHT OUTER JOIN
             Centres AS ct ON cp1.CustomPromptID = ct.CustomField1PromptID LEFT OUTER JOIN
             CustomPrompts AS cp2 ON ct.CustomField2PromptID = cp2.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp3 ON ct.CustomField3PromptID = cp3.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp4 ON ct.CustomField4PromptID = cp4.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp5 ON ct.CustomField5PromptID = cp5.CustomPromptID ON cp6.CustomPromptID = ct.CustomField6PromptID FULL OUTER JOIN
             JobGroups AS jg RIGHT OUTER JOIN
             Candidates AS c ON jg.JobGroupID = c.JobGroupID ON ct.CentreID = c.CentreID ON sd.CandidateID = c.CandidateID ";
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
                    (SELECT COALESCE
                    ((SELECT COUNT(casv.ID) AS Expr1
                    FROM    CandidateAssessmentSupervisors AS cas INNER JOIN
                               CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                               SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                               CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
                    WHERE (sd.SupervisorAdminID = @adminId) AND ((ca.RemovedDate IS NULL)) AND (casv.Verified IS NULL)
                    GROUP BY casv.Verified, casv.ID), 0)) AS AwaitingReviewCount", new { adminId }
                ).FirstOrDefault();
        }

        public IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId)
        {
            return connection.Query<SupervisorDelegateDetail>(
                $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.SupervisorAdminID = @adminId) AND (Removed IS NULL)
                    ORDER BY c.LastName, COALESCE(c.FirstName, sd.DelegateEmail)", new { adminId }
                );
        }
        public int AddSuperviseDelegate(int? supervisorAdminId, int? delegateId, string delegateEmail, string supervisorEmail, int centreId)
        {
            var addedByDelegate = (delegateId != null);
            if (delegateEmail.Length == 0 | supervisorEmail.Length == 0)
            {
                logger.LogWarning(
                    $"Not adding delegate to SupervisorDelegates as it failed server side valiidation. supervisorAdminId: {supervisorAdminId}, delegateEmail: {delegateEmail}"
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

        public SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId)
        {
            return connection.Query<SupervisorDelegateDetail>(
               $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.ID = @supervisorDelegateId) AND (Removed IS NULL)", new { supervisorDelegateId }
               ).FirstOrDefault();
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
                @"SELECT ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, COALESCE (sasr.RoleName, 'Supervisor') AS SupervisorRoleTitle, ca.StartedDate, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, r.RoleProfile, sg.SubGroup, pg.ProfessionalGroup,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS VerificationRequested,
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
        public DelegateSelfAssessment GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId)
        {
            return connection.Query<DelegateSelfAssessment>(
               @"SELECT ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, ca.StartedDate, COALESCE(ca.LastAccessed, ca.StartedDate) AS LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS VerificationRequested,
                 (SELECT COUNT(*) AS Expr1
                    FROM   SelfAssessmentResultSupervisorVerifications AS sarsv
                    WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Verified IS NULL)) AS ResultsVerificationRequests
                FROM CandidateAssessmentSupervisors AS cas INNER JOIN
                         CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                SelfAssessments AS sa ON sa.ID = ca.SelfAssessmentID 
                WHERE (ca.ID = @candidateAssessmentId)", new { candidateAssessmentId }
               ).FirstOrDefault();
        }
        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItems(int adminId)
        {
            return connection.Query<SupervisorDashboardToDoItem>(
                @"SELECT ca.ID, sd.ID AS SupervisorDelegateId, c.FirstName + ' ' + c.LastName AS DelegateName, sa.Name AS ProfileName, casv.Requested
                    FROM   CandidateAssessmentSupervisors AS cas INNER JOIN
                    CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                    SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                    CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID INNER JOIN
                    Candidates AS c ON ca.CandidateID = c.CandidateID
                WHERE (sd.SupervisorAdminID = @adminId) AND (casv.Verified IS NULL)", new { adminId }
                );
        }

        public DelegateSelfAssessment GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId)
        {
            return connection.Query<DelegateSelfAssessment>(
                @"SELECT ca.ID, sa.ID AS SelfAssessmentID, sa.Name AS RoleName, sa.SupervisorSelfAssessmentReview, sa.SupervisorResultsReview, COALESCE (sasr.RoleName, 'Supervisor') AS SupervisorRoleTitle, ca.StartedDate, ca.LastAccessed, ca.CompleteByDate, ca.LaunchCount, ca.CompletedDate, r.RoleProfile, sg.SubGroup, pg.ProfessionalGroup,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessmentSupervisorVerifications AS casv
                 WHERE (CandidateAssessmentSupervisorID = cas.ID) AND (Requested IS NOT NULL) AND (Verified IS NULL)) AS VerificationRequested,
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
                WHERE (ca.ID = @candidateAssessmentId) AND (sd.SupervisorAdminID = @adminId)", new { candidateAssessmentId, adminId }
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
                    WHERE SelfAssessmentResultSupervisorVerifications.ID = @selfAssessmentResultSupervisorVerificationId AND sd.SupervisorAdminID = @adminId  AND (Comments <> @comments OR SignedOff <> @signedOff)",
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
        public IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int candidateId, int centreId)
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
                 WHERE (CandidateID = @candidateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)))", new { candidateId, centreId }
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
        public int InsertCandidateAssessmentSupervisor(int delegateId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId)
        {
            int candidateAssessmentId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE
                 ((SELECT ID
                  FROM    CandidateAssessments
                   WHERE (SelfAssessmentID = @selfAssessmentId) AND (CandidateID = @delegateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)), 0) AS CandidateAssessmentID",
               new { selfAssessmentId, delegateId });
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
    }
}
