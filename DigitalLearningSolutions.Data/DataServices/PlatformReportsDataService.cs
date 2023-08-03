namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    public interface IPlatformReportsDataService
    {
        PlatformUsageSummary GetPlatformUsageSummary();
        IEnumerable<SelfAssessmentActivity> GetNursingProficienciesActivity(
            int? centreId,
            int? centreTypeId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? brandId,
            int? regionId,
            int? selfAssessmentId);
        DateTime GetNursingProficienciesActivityStartDate();
    }
    public class PlatformReportsDataService : IPlatformReportsDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<PlatformReportsDataService> logger;
        private readonly string selectSelfAssessmentActivity = @"SELECT al.ActivityDate, al.Enrolled, al.Submitted | al.SignedOff AS Completed
                                                                    FROM   ReportSelfAssessmentActivityLog AS al INNER JOIN
                                                                                     Centres AS ce ON al.CentreID = ce.CentreID
                                                                        WHERE (@endDate IS NULL OR
                                                                                     al.ActivityDate <= @endDate) AND (@jobGroupId IS NULL OR
                                                                                     al.JobGroupID = @jobGroupId) AND (al.ActivityDate >= @startDate) AND (@centreId IS NULL OR
                                                                                     al.CentreID = @centreId) AND (@regionId IS NULL OR ce.RegionID = @regionId) AND (@centreTypeID IS NULL OR ce.CentreTypeID = @centreTypeID)";
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
        public IEnumerable<SelfAssessmentActivity> GetNursingProficienciesActivity(
            int? centreId,
            int? centreTypeId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? brandId,
            int? regionId,
            int? selfAssessmentId)
        {
           return connection.Query<SelfAssessmentActivity>(
                 $@"{selectSelfAssessmentActivity} AND (@selfAssessmentId IS NULL OR
                                 al.SelfAssessmentID = @selfAssessmentId) AND (@courseCategoryId IS NULL OR
                                 al.CategoryID = @courseCategoryId) ",
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
        public IEnumerable<SelfAssessmentActivity> GetDigitalCapabilitiesActivity(
           int? centreId,
           int? centreTypeId,
           DateTime startDate,
           DateTime? endDate,
           int? jobGroupId,
           int? regionId)
        {
            return connection.Query<SelfAssessmentActivity>(
                  $@"{selectSelfAssessmentActivity} AND (@selfAssessmentId = 1) ",
                  new
                  {
                      centreId,
                      centreTypeId,
                      startDate,
                      endDate,
                      jobGroupId,
                      regionId
                  }
              );
        }
        public DateTime GetNursingProficienciesActivityStartDate()
        {
            return connection.QuerySingleOrDefault<DateTime>(
                @"SELECT MIN(ActivityDate) AS StartDate
                    FROM   ReportSelfAssessmentActivityLog
                    WHERE (SelfAssessmentID > 1)"
                );
        }
    }
}
