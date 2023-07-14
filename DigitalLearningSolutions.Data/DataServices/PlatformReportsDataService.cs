﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Models.User;
    using DocumentFormat.OpenXml.Wordprocessing;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data;
    public interface IPlatformReportsDataService
    {
        PlatformUsageSummary GetPlatformUsageSummary();
        IEnumerable<SelfAssessmentActivity> GetNursingProficienciesActivity(
            int? centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
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
                                                                                     al.CentreID = @centreId) AND (@regionId IS NULL OR ce.RegionID = @regionId)";
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
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessments WITH (NOLOCK)
                 WHERE (SelfAssessmentID = 1)) AS DCSAEnrolments,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessments AS CandidateAssessments_2 WITH (NOLOCK)
                 WHERE (SelfAssessmentID = 1) AND (SubmittedDate IS NOT NULL)) AS DCSACompletions,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessments AS CandidateAssessments_1 WITH (NOLOCK)
                 WHERE (SelfAssessmentID > 1)) AS NursingPassportEnrolments,
                 (SELECT COUNT(*) AS Expr1
                 FROM    CandidateAssessments AS ca WITH (NOLOCK) INNER JOIN
                              CandidateAssessmentSupervisors AS cas WITH (NOLOCK) ON ca.ID = cas.CandidateAssessmentID INNER JOIN
                              CandidateAssessmentSupervisorVerifications AS casv WITH (NOLOCK) ON cas.ID = casv.CandidateAssessmentSupervisorID AND casv.SignedOff = 1
                 WHERE (ca.SelfAssessmentID > 1)) AS NursingPassportCompletions"
            );
        }
        public IEnumerable<SelfAssessmentActivity> GetNursingProficienciesActivity(
            int? centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
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
                     startDate,
                     endDate,
                     jobGroupId,
                     selfAssessmentId,
                     courseCategoryId,
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
