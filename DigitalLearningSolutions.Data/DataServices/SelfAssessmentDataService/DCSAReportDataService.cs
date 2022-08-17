﻿namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
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
        public IEnumerable<DCSAOutcomeSummary> GetOutcomeSummaryForCentre(int centreId)
        {
            return connection.Query<DCSAOutcomeSummary>(
                @"SELECT DATEPART(month, caa.StartedDate) AS EnrolledMonth, DATEPART(yyyy, caa.StartedDate) AS EnrolledYear, jg.JobGroupName AS JobGroup, ca.Answer1 AS CentreField1, ca.Answer2 AS CentreField2, ca.Answer3 AS CentreField3, CASE WHEN (caa.SubmittedDate IS NOT NULL) 
                             THEN 'Submitted' WHEN (caa.UserBookmark LIKE N'/LearningPortal/SelfAssessment/1/Review' AND caa.SubmittedDate IS NULL) THEN 'Reviewing' ELSE 'Incomplete' END AS Status,
                                 (SELECT COUNT(*) AS Expr1
                                 FROM    FilteredLearningActivity AS fla
                                 WHERE (CandidateId = ca.CandidateID)) AS LearningLaunched,
                                 (SELECT COUNT(*) AS Expr1
                                 FROM    FilteredLearningActivity AS fla
                                 WHERE (CandidateId = ca.CandidateID) AND (CompletedDate IS NOT NULL)) AS LearningCompleted,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 1) AND (sar.CandidateID = ca.CandidateID)) AS DataInformationAndContentConfidence,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 1) AND (sar.CandidateID = ca.CandidateID)) AS DataInformationAndContentRelevance,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 2) AND (sar.CandidateID = ca.CandidateID)) AS TeachingLearningAndSelfDevelopmentConfidence,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                WHERE (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 2) AND (sar.CandidateID = ca.CandidateID)) AS TeachingLearningAndSelfDevelopmentRelevance,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 3) AND (sar.CandidateID = ca.CandidateID)) AS CommunicationCollaborationParticipationConfidence,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 3) AND (sar.CandidateID = ca.CandidateID)) AS CommunicationCollaborationParticipationRelevance,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 4) AND (sar.CandidateID = ca.CandidateID)) AS TechnicalProficiencyConfidence,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 4) AND (sar.CandidateID = ca.CandidateID)) AS TechnicalProficiencyRelevance,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 5) AND (sar.CandidateID = ca.CandidateID)) AS CreationInnovationResearchConfidence,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 5) AND (sar.CandidateID = ca.CandidateID)) AS CreationInnovationResearchRelevance,
                                 (SELECT AVG(sar.Result) AS AvgConfidence
                                 FROM    SelfAssessmentResults AS sar INNER JOIN
                                              Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                              SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 1) AND (sas.CompetencyGroupID = 6) AND (sar.CandidateID = ca.CandidateID)) AS DigitalIdentityWellbeingSafetyAndSecurityConfidence,
                                (SELECT AVG(sar.Result) AS AvgConfidence
                                FROM    SelfAssessmentResults AS sar INNER JOIN
                                             Competencies AS co ON sar.CompetencyID = co.ID INNER JOIN
                                             SelfAssessmentStructure AS sas ON co.ID = sas.CompetencyID
                                 WHERE (sar.AssessmentQuestionID = 2) AND (sas.CompetencyGroupID = 6) AND (sar.CandidateID = ca.CandidateID)) AS DigitalIdentityWellbeingSafetyAndSecurityRelevance
                FROM   Candidates AS ca INNER JOIN
                             CandidateAssessments AS caa ON ca.CandidateID = caa.CandidateID INNER JOIN
                             JobGroups AS jg ON ca.JobGroupID = jg.JobGroupID
                WHERE (ca.Active = 1) AND (ca.CentreID = @centreId) AND (caa.SelfAssessmentID = 1)
                ORDER BY EnrolledYear DESC, EnrolledMonth DESC, JobGroup, CentreField1, CentreField2, CentreField3, Status",
                new { centreId }
            );
        }

        public IEnumerable<DCSADelegateCompletionStatus> GetDelegateCompletionStatusForCentre(int centreId)
        {
            return connection.Query<DCSADelegateCompletionStatus>(
                @"SELECT DATEPART(month, caa.StartedDate) AS EnrolledMonth, DATEPART(yyyy, caa.StartedDate) AS EnrolledYear, ca.FirstName, ca.LastName, ca.EmailAddress AS Email, ca.Answer1 AS CentreField1, ca.Answer2 AS CentreField2, ca.Answer3 AS CentreField3, CASE WHEN (caa.SubmittedDate IS NOT NULL) 
                             THEN 'Submitted' WHEN (caa.UserBookmark LIKE N'/LearningPortal/SelfAssessment/1/Review' AND caa.SubmittedDate IS NULL) THEN 'Reviewing' ELSE 'Incomplete' END AS Status
                FROM   Candidates AS ca INNER JOIN
                             CandidateAssessments AS caa ON ca.CandidateID = caa.CandidateID INNER JOIN
                             JobGroups AS jg ON ca.JobGroupID = jg.JobGroupID
                WHERE (ca.Active = 1) AND (ca.CentreID = @centreId) AND (caa.SelfAssessmentID = 1)
                ORDER BY EnrolledYear DESC, EnrolledMonth DESC, ca.LastName, ca.FirstName",
                new { centreId }
            );
        }
    }
}
