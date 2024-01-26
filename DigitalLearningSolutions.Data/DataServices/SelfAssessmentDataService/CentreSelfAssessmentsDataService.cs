namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SuperAdmin;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface ICentreSelfAssessmentsDataService
    {
        IEnumerable<CentreSelfAssessment> GetCentreSelfAssessments(int centreId);
        CentreSelfAssessment? GetCentreSelfAssessmentByCentreAndID(int centreId, int selfAssessmentId);
        IEnumerable<SelfAssessmentForPublish> GetCentreSelfAssessmentsForPublish(int centreId);
        void DeleteCentreSelfAssessment(int centreId, int selfAssessmentId);
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
        public CentreSelfAssessment? GetCentreSelfAssessmentByCentreAndID(int centreId, int selfAssessmentId)
        {
            var centreSelfAssessment = connection.QueryFirstOrDefault<CentreSelfAssessment?>(
                @"SELECT TOP (1) csa.SelfAssessmentID, csa.CentreID, Centres.CentreName, sa.Name AS SelfAssessmentName,
                                 (SELECT COUNT(1) AS DelegateCount
                                 FROM    CandidateAssessments AS ca INNER JOIN
                                              Users AS u ON ca.DelegateUserID = u.ID INNER JOIN
                                              DelegateAccounts AS da ON da.UserID = u.ID
                                 WHERE (ca.SelfAssessmentID = sa.ID) AND (da.CentreID = @centreId) AND (ca.RemovedDate IS NULL) AND (u.Active = 1) AND (da.Active = 1)) AS DelegateCount, csa.AllowEnrolment AS SelfEnrol
                    FROM   CentreSelfAssessments AS csa INNER JOIN
                             SelfAssessments AS sa ON csa.SelfAssessmentID = sa.ID INNER JOIN
                             Centres ON csa.CentreID = Centres.CentreID
                    WHERE (csa.CentreID = @centreId) AND (csa.SelfAssessmentID = @selfAssessmentId) AND (sa.ArchivedDate IS NULL)
                    ORDER BY SelfAssessmentName",
                 new { centreId, selfAssessmentId }
                );
            if (centreSelfAssessment == null)
            {
                logger.LogWarning($"No centre self assessment found for centre id {centreId} and application id {selfAssessmentId}");
            }
            return centreSelfAssessment;
        }
        public IEnumerable<SelfAssessmentForPublish> GetCentreSelfAssessmentsForPublish(int centreId)
        {
            return connection.Query<SelfAssessmentForPublish>(
                @"SELECT sa.SelfAssessmentID, sa.Name AS SelfAssessmentName, sa.National, b.BrandName AS Provider
                    FROM SelfAssessments AS sa LEFT OUTER JOIN Brands AS b ON sa.BrandID = b.BrandID
                    WHERE (sa.ArchivedDate IS NULL)
                    AND (sa.PublishStatusID = 3)
                    AND (sa.SelfAssessmentID NOT IN
                        (SELECT SelfAssessmentID
                            FROM CentreSelfAssessments AS csa
                            WHERE csa.CentreID = @centreId))
                    ORDER BY SelfAssessmentName", new { centreId }
                );
        }
        public void DeleteCentreSelfAssessment(int centreId, int selfAssessmentId)
        {
            connection.Execute(
                    @"DELETE 
                            FROM   CentreSelfAssessments
                            WHERE (CentreID = @centreId) AND (SelfAssessmentID = @selfAssessmentId)"
            ,
                    new { centreId, selfAssessmentId }
                );
        }
    }
}
