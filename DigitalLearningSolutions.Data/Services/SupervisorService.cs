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
                  WHERE (sd.SupervisorAdminID = @adminId) AND (NOT (ca.RemovedDate IS NULL))
                  GROUP BY casv.Verified, casv.ID
                  HAVING (NOT (casv.Verified IS NULL))), 0)) AS AwaitingReviewCount", new { adminId }
                ).FirstOrDefault();
        }
    }
}
