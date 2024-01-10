namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data;

    public interface ICentreSelfAssessmentsDataService
    {
        IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId);
    }
    public class CentreSelfAssessmentsDataService : ICentreSelfAssessmentsDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentDataService> logger;
        public CentreSelfAssessmentsDataService(IDbConnection connection, ILogger<SelfAssessmentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        public IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId)
        {
            return connection.Query<CentreSelfAssessment>(
                @"SELECT csa.SelfAssessmentID, csa.CentreID, sa.Name AS SelfAssessmentName,
                                 (SELECT COUNT(1) AS DelegateCount
                                 FROM    CandidateAssessments AS ca INNER JOIN
                                                  Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                                  DelegateAccounts AS da ON da.UserID = u.ID
                                     WHERE (ca.SelfAssessmentID = sa.ID) AND (da.CentreID = @centreId) AND (ca.RemovedDate IS NULL) AND (u.Active = 1) AND (da.Active = 1)) AS DelegateCount, csa.AllowEnrolment AS SelfEnrol
                    FROM   CentreSelfAssessments AS csa INNER JOIN
                                 SelfAssessments AS sa ON csa.SelfAssessmentID = sa.ID
                    WHERE (csa.CentreID = @centreId) AND (sa.ArchivedDate IS NULL)
                    ORDER BY SelfAssessmentName", new { centreId }
                );
        }
    }
}
