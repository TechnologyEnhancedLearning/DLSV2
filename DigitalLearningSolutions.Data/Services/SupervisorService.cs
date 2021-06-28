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
        //UPDATE DATA

        //INSERT DATA

        //DELETE DATA


    }
    public class SupervisorService : ISupervisorService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SupervisorService> logger;
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
                @"SELECT sd.ID, sd.SupervisorEmail, sd.SupervisorAdminID, sd.DelegateEmail, sd.CandidateID, sd.Added, sd.AddedByDelegate, sd.NotificationSent, sd.Confirmed, c.FirstName, c.LastName, jg.JobGroupName, c.Answer1, c.Answer2, c.Answer3, c.Answer4, c.Answer5, c.Answer6, 
             cp1.CustomPrompt AS CustomPrompt1, cp2.CustomPrompt AS CustomPrompt2, cp3.CustomPrompt AS CustomPrompt3, cp4.CustomPrompt AS CustomPrompt4, cp5.CustomPrompt AS CustomPrompt5, cp6.CustomPrompt AS CustomPrompt6
FROM   Centres AS ct LEFT OUTER JOIN
             CustomPrompts AS cp1 ON ct.CustomField1PromptID = cp1.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp2 ON ct.CustomField1PromptID = cp2.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp3 ON ct.CustomField1PromptID = cp3.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp4 ON ct.CustomField1PromptID = cp4.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp5 ON ct.CustomField1PromptID = cp5.CustomPromptID LEFT OUTER JOIN
             CustomPrompts AS cp6 ON ct.CustomField1PromptID = cp6.CustomPromptID RIGHT OUTER JOIN
             Candidates AS c ON ct.CentreID = c.CentreID RIGHT OUTER JOIN
             SupervisorDelegates AS sd ON c.CandidateID = sd.CandidateID LEFT OUTER JOIN
             JobGroups AS jg ON c.JobGroupID = jg.JobGroupID
WHERE (sd.SupervisorAdminID = @adminId)", new {adminId}
                );
        }
    }
}
