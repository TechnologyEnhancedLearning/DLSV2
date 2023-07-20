namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using Microsoft.Extensions.Logging;
    using System.Data;
    public interface IPlatformReportsDataService
    {
        PlatformUsageSummary GetPlatformUsageSummary();
    }
    public class PlatformReportsDataService : IPlatformReportsDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<PlatformReportsDataService> logger;
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
    }
}
