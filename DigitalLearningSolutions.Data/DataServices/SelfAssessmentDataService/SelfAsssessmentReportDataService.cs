namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data;
    using ClosedXML.Excel;

    public interface ISelfAssessmentReportDataService
    {
        IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId);
        IEnumerable<SelfAssessmentReportData> GetSelfAssessmentReportDataForCentre(int centreId, int selfAssessmentId);
    }
    public partial class SelfAssessmentReportDataService : ISelfAssessmentReportDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentReportDataService> logger;

        public SelfAssessmentReportDataService(IDbConnection connection, ILogger<SelfAssessmentReportDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId)
        {
            return connection.Query<SelfAssessmentSelect>(
                  @"SELECT csa.SelfAssessmentID AS Id, sa.Name,
                                   (SELECT COUNT (DISTINCT da.UserID) AS Learners
                                   FROM   CandidateAssessments AS ca1 INNER JOIN
                                                DelegateAccounts AS da ON ca1.DelegateUserID = da.UserID
                                   WHERE (da.CentreID = @centreId) AND (ca1.RemovedDate IS NULL) AND (ca1.SelfAssessmentID = csa.SelfAssessmentID) AND ca1.NonReportable=0) AS LearnerCount
                  FROM   CentreSelfAssessments AS csa INNER JOIN
                               SelfAssessments AS sa ON csa.SelfAssessmentID = sa.ID
                  WHERE (csa.CentreID = @centreId) AND (sa.CategoryID = @categoryId) AND (sa.SupervisorResultsReview = 1) AND (sa.ArchivedDate IS NULL) OR
                               (csa.CentreID = @centreId) AND (sa.CategoryID = @categoryId) AND (sa.ArchivedDate IS NULL) AND (sa.SupervisorSelfAssessmentReview = 1) OR
                               (csa.CentreID = @centreId) AND (sa.SupervisorResultsReview = 1) AND (sa.ArchivedDate IS NULL) AND (@categoryId = 0) OR
                               (csa.CentreID = @centreId) AND (sa.ArchivedDate IS NULL) AND (sa.SupervisorSelfAssessmentReview = 1) AND (@categoryId = 0)
                  ORDER BY sa.Name",
                new { centreId, categoryId = categoryId ??= 0 }
            );
        }

        public IEnumerable<SelfAssessmentReportData> GetSelfAssessmentReportDataForCentre(int centreId, int selfAssessmentId)
        {
            return connection.Query<SelfAssessmentReportData>("usp_GetSelfAssessmentReport",
                new { selfAssessmentId, centreId },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 150
            );
        }
    }
}
