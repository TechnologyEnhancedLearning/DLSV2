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
    public interface ISupervisorService
    {
        //GET DATA
        DashboardData GetDashboardDataForAdminId(int adminId);
        IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId);
        SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId);
        //UPDATE DATA
        bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool RemoveSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        //INSERT DATA
        int AddSuperviseDelegate(int supervisorAdminId, string delegateEmail, string supervisorEmail, int centreId);
        //DELETE DATA


    }
    public class SupervisorService : ISupervisorService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorService> logger;
        private const string supervisorDelegateDetailFields = @"sd.ID, sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateEmail, sd.CandidateID, sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Confirmed, sd.Removed, c.FirstName, c.LastName, jg.JobGroupName, c.Answer1, c.Answer2, c.Answer3, c.Answer4, c.Answer5, c.Answer6, 
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
                    WHERE (sd.SupervisorAdminID = @adminId) AND (NOT (ca.RemovedDate IS NULL))) AS ProfileSelfAssessmentCount, (SELECT COALESCE
                    ((SELECT COUNT(casv.ID) AS Expr1
                    FROM    CandidateAssessmentSupervisors AS cas INNER JOIN
                               CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID INNER JOIN
                               SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                               CandidateAssessmentSupervisorVerifications AS casv ON cas.ID = casv.CandidateAssessmentSupervisorID
                    WHERE (sd.SupervisorAdminID = @adminId) AND (NOT (ca.RemovedDate IS NULL)) AND (NOT (casv.Verified IS NULL))
                    GROUP BY casv.Verified, casv.ID), 0)) AS AwaitingReviewCount", new { adminId }
                ).FirstOrDefault();
        }

        public IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId)
        {
            return connection.Query<SupervisorDelegateDetail>(
                $@"SELECT {supervisorDelegateDetailFields}
                    FROM   {supervisorDelegateDetailTables}
                    WHERE (sd.SupervisorAdminID = @adminId) AND (Removed IS NULL)", new {adminId}
                );
        }
        public int AddSuperviseDelegate(int supervisorAdminId, string delegateEmail, string supervisorEmail, int centreId)
        {
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
                  WHERE (SupervisorAdminID = @supervisorAdminId) AND (DelegateEmail = @delegateEmail)), 0) AS ID",
               new { supervisorAdminId, delegateEmail });
            if (existingId > 0)
            {
                var numberOfAffectedRows = connection.Execute(@"UPDATE SupervisorDelegates SET Removed = NULL WHERE (SupervisorAdminID = @supervisorAdminId) AND (DelegateEmail = @delegateEmail) AND (Removed IS NOT NULL)", new { supervisorAdminId, delegateEmail });
                if (numberOfAffectedRows > 0)
                {
                    return existingId;
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                var delegateId = (int?)connection.ExecuteScalar(
                    @"SELECT CandidateID FROM Candidates WHERE EmailAddress = @delegateEmail AND Active = 1 AND CentreID = @centreId", new { delegateEmail, centreId }
                    );

                var numberOfAffectedRows = connection.Execute(
         @"INSERT INTO SupervisorDelegates (SupervisorAdminID, DelegateEmail, CandidateID, SupervisorEmail)
                    VALUES (@supervisorAdminId, @delegateEmail, @delegateId, @supervisorEmail)",
        new { supervisorAdminId, delegateEmail, delegateId, supervisorEmail });
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
                  WHERE (SupervisorAdminID = @supervisorAdminId) AND (DelegateEmail = @delegateEmail)), 0) AS AdminID",
               new { supervisorAdminId, delegateEmail });
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
    }
}
