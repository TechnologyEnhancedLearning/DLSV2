namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    public interface IPlatformReportsDataService
    {
        PlatformUsageSummary GetPlatformUsageSummary();
        IEnumerable<SelfAssessmentActivity> GetSelfAssessmentActivity(
            int? centreId,
            int? centreTypeId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? brandId,
            int? regionId,
            int? selfAssessmentId,
            bool supervised);
        DateTime GetSelfAssessmentActivityStartDate(bool supervised);
    }
    public class PlatformReportsDataService : IPlatformReportsDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<PlatformReportsDataService> logger;
        private readonly string selectSelfAssessmentActivity = @"SELECT al.ActivityDate, al.Enrolled, al.Submitted | al.SignedOff AS Completed
                                                                    FROM   ReportSelfAssessmentActivityLog AS al INNER JOIN
                                                                                     Centres AS ce ON al.CentreID = ce.CentreID INNER JOIN
                                                                                     SelfAssessments AS sa ON sa.ID = al.SelfAssessmentID
                                                                        WHERE (@endDate IS NULL OR al.ActivityDate <= @endDate) AND
                                                                                     (al.ActivityDate >= @startDate) AND
                                                                                     (sa.[National] = 1) AND
                                                                                     (sa.ArchivedDate IS NULL) AND
                                                                                     (@jobGroupId IS NULL OR al.JobGroupID = @jobGroupId) AND
                                                                                     (@centreId IS NULL OR al.CentreID = @centreId) AND
                                                                                     (@regionId IS NULL OR ce.RegionID = @regionId) AND
                                                                                     (@centreTypeID IS NULL OR ce.CentreTypeID = @centreTypeID) AND
                                                                                     (@selfAssessmentId IS NULL OR al.SelfAssessmentID = @selfAssessmentId) AND
                                                                                     (@courseCategoryId IS NULL OR al.CategoryID = @courseCategoryId) AND
                                                                                     (@brandId IS NULL OR sa.BrandID = @brandId)";
        private string GetSelfAssessmentWhereClause(bool supervised)
        {
            return supervised ? " (sa.SupervisorResultsReview = 1 OR SupervisorSelfAssessmentReview = 1)" : " (sa.SupervisorResultsReview = 0 AND SupervisorSelfAssessmentReview = 0)";
        }
        public PlatformReportsDataService(IDbConnection connection)
        {
            this.connection = connection;
            this.logger = logger;
        }
        public PlatformUsageSummary GetPlatformUsageSummary()
        {
            return connection.QueryFirstOrDefault<PlatformUsageSummary>(
            @"SELECT (SELECT COUNT(CentreName) AS ActiveCentres
              FROM    Centres AS Centres_1 WITH (NOLOCK)
              WHERE  (Active = 1) AND (AutoRegistered = 1)) AS ActiveCentres,
                 (SELECT COUNT(SessionID) AS LearnerLogins
                 FROM    Sessions AS e WITH (NOLOCK)) AS LearnerLogins,
                 (SELECT COUNT(ID) AS Learners
                 FROM    DelegateAccounts WITH (NOLOCK)
                 WHERE (Active = 1)) AS Learners,
                 (SELECT SUM(Duration) AS CourseLearningTime
                 FROM    Sessions AS e WITH (NOLOCK)) / 60 AS CourseLearningTime,
				 (SELECT COUNT(ProgressID) AS CourseEnrolments
                 FROM    Progress AS P WITH (NOLOCK)) AS CourseEnrolments,
                 (SELECT COUNT(ProgressID) AS CourseCompletions
                 FROM    Progress AS P WITH (NOLOCK)
                 WHERE (Completed IS NOT NULL)) AS CourseCompletions,
                 (SELECT COUNT(ID) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog WITH (NOLOCK)
                 WHERE (SelfAssessmentID = 1) AND (Enrolled=1)) AS DCSAEnrolments,
                 (SELECT COUNT(ID) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog WITH (NOLOCK)
                 WHERE (SelfAssessmentID = 1) AND (Submitted = 1)) AS DCSACompletions,
                 (SELECT COUNT(*) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog WITH (NOLOCK)
                 WHERE (SelfAssessmentID > 1) AND (Enrolled=1)) AS NursingPassportEnrolments,
                 (SELECT COUNT(*) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog
                 WHERE (SelfAssessmentID > 1) AND (SignedOff = 1)) AS NursingPassportCompletions"
            );
        }
        public IEnumerable<SelfAssessmentActivity> GetSelfAssessmentActivity(
            int? centreId,
            int? centreTypeId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? brandId,
            int? regionId,
            int? selfAssessmentId,
            bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
           return connection.Query<SelfAssessmentActivity>(
                 $@"{selectSelfAssessmentActivity} AND {whereClause}",
                 new
                 {
                     centreId,
                     centreTypeId,
                     startDate,
                     endDate,
                     jobGroupId,
                     selfAssessmentId,
                     courseCategoryId,
                     brandId,
                     regionId
                 }
             );
        }
        public DateTime GetSelfAssessmentActivityStartDate(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.QuerySingleOrDefault<DateTime>(
                $@"SELECT MIN(al.ActivityDate) AS StartDate
                    FROM   ReportSelfAssessmentActivityLog AS al INNER JOIN
                           SelfAssessments AS sa ON sa.ID = al.SelfAssessmentID
                    WHERE  {whereClause}"
                );
        }
        
    }
}
