namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
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
        IEnumerable<ActivityLog> GetFilteredCourseActivity(
            int? centreId,
            int? centreTypeId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? brandId,
            int? regionId,
            int? applicationId,
            bool? coreContent
        );
        DateTime? GetStartOfCourseActivity();
    }
    public class PlatformReportsDataService : IPlatformReportsDataService
    {
        private readonly IDbConnection connection;
        private readonly string selectSelfAssessmentActivity = @"SELECT Cast(al.ActivityDate As Date) As ActivityDate, SUM(CAST(al.Enrolled AS Int)) AS Enrolled, SUM(CAST((al.Submitted | al.SignedOff) AS Int)) AS Completed
                                                                    FROM   ReportSelfAssessmentActivityLog AS al WITH (NOLOCK) INNER JOIN
                                                                                     Centres AS ce WITH (NOLOCK) ON al.CentreID = ce.CentreID INNER JOIN
                                                                                     SelfAssessments AS sa WITH (NOLOCK) ON sa.ID = al.SelfAssessmentID
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
                 (SELECT COUNT(*) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog AS al WITH (NOLOCK) INNER JOIN
                 SelfAssessments AS sa WITH (NOLOCK) ON sa.ID = al.SelfAssessmentID
                 WHERE (sa.SupervisorResultsReview = 0 AND SupervisorSelfAssessmentReview = 0) AND (Enrolled=1)) AS IndependentSelfAssessmentEnrolments,
                 (SELECT COUNT(*) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog AS al WITH (NOLOCK) INNER JOIN
                 SelfAssessments AS sa WITH (NOLOCK) ON sa.ID = al.SelfAssessmentID
                 WHERE (sa.SupervisorResultsReview = 0 AND SupervisorSelfAssessmentReview = 0) AND (Submitted = 1)) AS IndependentSelfAssessmentCompletions,
                 (SELECT COUNT(*) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog AS al WITH (NOLOCK) INNER JOIN
                 SelfAssessments AS sa WITH (NOLOCK) ON sa.ID = al.SelfAssessmentID
                 WHERE (sa.SupervisorResultsReview = 1 OR SupervisorSelfAssessmentReview = 1) AND (Enrolled=1)) AS SupervisedSelfAssessmentEnrolments,
                 (SELECT COUNT(*) AS Expr1
                 FROM    ReportSelfAssessmentActivityLog AS al WITH (NOLOCK) INNER JOIN
                 SelfAssessments AS sa WITH (NOLOCK) ON sa.ID = al.SelfAssessmentID
                 WHERE (sa.SupervisorResultsReview = 1 OR SupervisorSelfAssessmentReview = 1) AND (SignedOff = 1)) AS SupervisedSelfAssessmentCompletions"
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
                  $@"{selectSelfAssessmentActivity} AND {whereClause} GROUP BY  Cast(al.ActivityDate As Date)",
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
                    FROM   ReportSelfAssessmentActivityLog AS al WITH (NOLOCK) INNER JOIN
                           SelfAssessments AS sa WITH (NOLOCK) ON sa.ID = al.SelfAssessmentID
                    WHERE  {whereClause}"
                );
        }

        public IEnumerable<ActivityLog> GetFilteredCourseActivity(
            int? centreId,
            int? centreTypeId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? brandId,
            int? regionId,
            int? applicationId,
            bool? coreContent
        )
        {
            return connection.Query<ActivityLog>(
                @"SELECT
                        Cast(LogDate As Date) As LogDate,
						LogYear,
                        LogQuarter,
                        LogMonth,
                        SUM(CAST(Registered AS Int)) AS Registered,
						SUM(CAST(Completed AS Int)) AS Completed,
						SUM(CAST(Evaluated AS Int)) AS Evaluated
                    FROM tActivityLog AS al WITH(NOLOCK) INNER JOIN
                         Applications AS ap WITH(NOLOCK) ON ap.ApplicationID = al.ApplicationID INNER JOIN
                         Centres AS ce  WITH(NOLOCK) ON al.CentreID = ce.CentreID
                    WHERE (ap.DefaultContentTypeID <> 4)
                        AND (al.LogDate >= @startDate)
                        AND (@endDate IS NULL OR al.LogDate <= @endDate)
                        AND (@centreId IS NULL OR al.CentreID = @centreId)
                        AND (@jobGroupId IS NULL OR al.JobGroupID = @jobGroupId)
                        AND (@regionId IS NULL OR al.RegionID = @regionId)
                        AND (@applicationId IS NULL OR al.ApplicationID = @applicationId)
                        AND (@courseCategoryId IS NULL OR al.CourseCategoryId = @courseCategoryId)
                        AND (@centreTypeID IS NULL OR ce.CentreTypeID = @centreTypeID)
                        AND (@brandId IS NULL OR al.BrandID = @brandId)
                        AND (al.Registered = 1 OR al.Completed = 1 OR al.Evaluated = 1)
                        AND (@coreContent IS NULL OR ap.CoreContent = @coreContent)
					GROUP BY  Cast(LogDate As Date), LogYear,
                        LogQuarter,
                        LogMonth",
                new
                {
                    centreId,
                    centreTypeId,
                    startDate,
                    endDate,
                    jobGroupId,
                    brandId,
                    regionId,
                    applicationId,
                    courseCategoryId,
                    coreContent
                }
            );
        }

        public DateTime? GetStartOfCourseActivity()
        {
            return connection.QuerySingleOrDefault<DateTime?>(
                @"SELECT MIN(LogDate)
                    FROM tActivityLog WITH (NOLOCK)"
            );
        }
    }
}
