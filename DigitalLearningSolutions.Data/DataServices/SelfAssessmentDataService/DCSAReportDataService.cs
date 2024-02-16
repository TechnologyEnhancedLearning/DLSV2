namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data;

    public interface IDCSAReportDataService
    {
        IEnumerable<DCSADelegateCompletionStatus> GetDelegateCompletionStatusForCentre(int centreId);
        IEnumerable<DCSAOutcomeSummary> GetOutcomeSummaryForCentre(int centreId);
    }
    public partial class DCSAReportDataService : IDCSAReportDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentDataService> logger;

        public DCSAReportDataService(IDbConnection connection, ILogger<SelfAssessmentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        
        public IEnumerable<DCSADelegateCompletionStatus> GetDelegateCompletionStatusForCentre(int centreId)
        {
            return connection.Query<DCSADelegateCompletionStatus>(
                @"SELECT DATEPART(month, ca.StartedDate) AS EnrolledMonth, DATEPART(yyyy, ca.StartedDate) AS EnrolledYear, u.FirstName, u.LastName, COALESCE (ucd.Email, u.PrimaryEmail) AS Email, da.Answer1 AS CentreField1, da.Answer2 AS CentreField2, da.Answer3 AS CentreField3, 
                        CASE WHEN (ca.SubmittedDate IS NOT NULL) THEN 'Submitted' WHEN (ca.UserBookmark LIKE N'/LearningPortal/SelfAssessment/1/Review' AND ca.SubmittedDate IS NULL) THEN 'Reviewing' ELSE 'Incomplete' END AS Status
                    FROM   CandidateAssessments AS ca INNER JOIN
                        DelegateAccounts AS da ON ca.DelegateUserID = da.UserID INNER JOIN
                        Users AS u ON da.UserID = u.ID LEFT OUTER JOIN
                        UserCentreDetails AS ucd ON da.CentreID = ucd.CentreID AND u.ID = ucd.UserID
                    WHERE (ca.SelfAssessmentID = 1) AND (da.CentreID = @centreId) AND (u.Active = 1) AND (da.Active = 1)",
                new { centreId }
            );
        }

        public IEnumerable<DCSAOutcomeSummary> GetOutcomeSummaryForCentre(int centreId)
        {
            return connection.Query<DCSAOutcomeSummary>(
                @"SELECT DATEPART(month, ca.StartedDate) AS EnrolledMonth, DATEPART(yyyy, ca.StartedDate) AS EnrolledYear, jg.JobGroupName AS JobGroup, da.Answer1 AS CentreField1, da.Answer2 AS CentreField2, da.Answer3 AS CentreField3, CASE WHEN (ca.SubmittedDate IS NOT NULL) 
                   THEN 'Submitted' WHEN (ca.UserBookmark LIKE N'/LearningPortal/SelfAssessment/1/Review' AND ca.SubmittedDate IS NULL) THEN 'Reviewing' ELSE 'Incomplete' END AS Status,
                     (SELECT COUNT(*) AS LearningLaunched
                     FROM    CandidateAssessmentLearningLogItems AS calli INNER JOIN
                                  LearningLogItems AS lli ON calli.LearningLogItemID = lli.LearningLogItemID
                     WHERE (NOT (lli.LearningResourceReferenceID IS NULL)) AND (calli.CandidateAssessmentID = ca.ID)) +
                     (SELECT COUNT(*) AS FilteredLearning
                     FROM    FilteredLearningActivity AS fla
                     WHERE (CandidateId = da.ID)) AS LearningCompleted,
                         (SELECT COUNT(*) AS LearningLaunched
                     FROM    CandidateAssessmentLearningLogItems AS calli INNER JOIN
                                  LearningLogItems AS lli ON calli.LearningLogItemID = lli.LearningLogItemID
                     WHERE (NOT (lli.LearningResourceReferenceID IS NULL)) AND (calli.CandidateAssessmentID = ca.ID) AND (NOT (lli.CompletedDate IS NULL))) +
                     (SELECT COUNT(*) AS FilteredCompleted
                     FROM    FilteredLearningActivity AS fla
                     WHERE (CandidateId = da.ID) AND (CompletedDate IS NOT NULL)) AS LearningCompleted,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                  Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                  SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 1)) AS DataInformationAndContentConfidence,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                  Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                  SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 1)) AS DataInformationAndContentRelevance,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 2)) AS TeachingLearningAndSelfDevelopmentConfidence,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 2)) AS TeachingLearningAndSelfDevelopmentRelevance,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 3)) AS CommunicationCollaborationAndParticipationConfidence,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                  SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                     WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 3)) AS CommunicationCollaborationAndParticipationRelevance,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 4)) AS TechnicalProficiencyConfidence,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 4)) AS TechnicalProficiencyRelevance,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 5)) AS CreationInnovationAndResearchConfidence,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 5)) AS CreationInnovationAndResearchRelevance,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 6)) AS DigitalIdentityWellbeingSafetyAndSecurityConfidence,
                    (SELECT AVG(sar.Result) AS AvgConfidence
                    FROM    SelfAssessmentResults AS sar INNER JOIN
                                Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                    WHERE (sar.DelegateUserID = da.UserID) AND (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 6)) AS DigitalIdentityWellbeingSafetyAndSecurityRelevance
                    FROM   CandidateAssessments AS ca INNER JOIN
                    DelegateAccounts AS da ON ca.DelegateUserID = da.UserID INNER JOIN
                    Users AS u ON da.UserID = u.ID INNER JOIN
                     JobGroups AS jg ON u.JobGroupID = jg.JobGroupID
                    WHERE (ca.SelfAssessmentID = 1) AND (da.CentreID = @centreId) AND (u.Active = 1) AND (da.Active = 1)",
                new { centreId }
            );
        }

    }
}
