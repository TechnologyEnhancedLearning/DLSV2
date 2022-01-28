namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System.Collections.Generic;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;

    public partial class SelfAssessmentDataService
    {
        public CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(int candidateAssessmentId, int candidateId)
        {
            return connection.QuerySingle<CandidateAssessmentExportSummary>(
                @"SELECT sa.Name AS SelfAssessment, c.FirstName + ' ' + c.LastName AS CandidateName, c.ProfessionalRegistrationNumber AS CandidatePrn, ca.StartedDate AS StartDate,
                 (SELECT COUNT(sas.CompetencyID) AS CompetencyAssessmentQuestionCount
                 FROM    SelfAssessmentStructure AS sas INNER JOIN
                              CompetencyAssessmentQuestions AS caq ON sas.CompetencyID = caq.CompetencyID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc ON sas.CompetencyID = caoc.CompetencyID AND sas.CompetencyGroupID = caoc.CompetencyGroupID AND ca.ID = caoc.CandidateAssessmentID
                 WHERE (sas.SelfAssessmentID = sa.ID) AND (sas.Optional = 0) OR
                              (sas.SelfAssessmentID = sa.ID) AND (caoc.IncludedInSelfAssessment = 1)) AS QuestionCount,
                 (SELECT COUNT(sas1.CompetencyID) AS ResultCount
                 FROM    SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID LEFT OUTER JOIN
                              SelfAssessmentResults AS sar1 ON sar1.ID =
                                  (SELECT MAX(ID) AS Expr1
                                  FROM    SelfAssessmentResults AS sar2
                                  WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS SelfAssessmentResponseCount,
                 (SELECT COUNT(sas1.CompetencyID) AS VerifiedCount
                 FROM    SelfAssessmentResultSupervisorVerifications INNER JOIN
                              SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications.SelfAssessmentResultId = sar1.ID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                                  (SELECT MAX(ID) AS Expr1
                                  FROM    SelfAssessmentResults AS sar2
                                  WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (SelfAssessmentResultSupervisorVerifications.SignedOff = 1) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS ResponsesVerifiedCount,
                 (SELECT COUNT(sas1.CompetencyID) AS NoRequirementsSetCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS SelfAssessmentResultSupervisorVerifications_4 INNER JOIN
                              SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications_4.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                                  (SELECT MAX(ID) AS Expr1
                                  FROM    SelfAssessmentResults AS sar2
                                  WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_4.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_4.SignedOff = 1) AND (caqrr1.ID IS NULL) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (SelfAssessmentResultSupervisorVerifications_4.SignedOff = 1) AND (caqrr1.ID IS NULL) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (SelfAssessmentResultSupervisorVerifications_4.SignedOff = 1) AND (caqrr1.ID IS NULL) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS NoRequirementsSetCount,
                 (SELECT COUNT(sas1.CompetencyID) AS NotMeetingCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS SelfAssessmentResultSupervisorVerifications_3 INNER JOIN
                              SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications_3.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                                  (SELECT MAX(ID) AS Expr1
                                  FROM    SelfAssessmentResults AS sar2
                                  WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_3.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_3.SignedOff = 1) AND (caqrr1.LevelRAG = 1) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (SelfAssessmentResultSupervisorVerifications_3.SignedOff = 1) AND (caqrr1.LevelRAG = 1) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (SelfAssessmentResultSupervisorVerifications_3.SignedOff = 1) AND (caqrr1.LevelRAG = 1) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS NotMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS PartiallyMeeting
                 FROM    SelfAssessmentResultSupervisorVerifications AS SelfAssessmentResultSupervisorVerifications_2 INNER JOIN
                              SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications_2.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                                  (SELECT MAX(ID) AS Expr1
                                  FROM    SelfAssessmentResults AS sar2
                                  WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_2.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_2.SignedOff = 1) AND (caqrr1.LevelRAG = 2) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (SelfAssessmentResultSupervisorVerifications_2.SignedOff = 1) AND (caqrr1.LevelRAG = 2) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (SelfAssessmentResultSupervisorVerifications_2.SignedOff = 1) AND (caqrr1.LevelRAG = 2) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS PartiallyMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS MeetingCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS SelfAssessmentResultSupervisorVerifications_1 INNER JOIN
                              SelfAssessmentResults AS sar1 ON SelfAssessmentResultSupervisorVerifications_1.SelfAssessmentResultId = sar1.ID LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.ID =
                                  (SELECT MAX(ID) AS Expr1
                                  FROM    SelfAssessmentResults AS sar2
                                  WHERE (CompetencyID = caq1.CompetencyID) AND (AssessmentQuestionID = caq1.AssessmentQuestionID) AND (CandidateID = ca1.CandidateID) AND (SelfAssessmentID = ca1.SelfAssessmentID)) LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_1.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (SelfAssessmentResultSupervisorVerifications_1.SignedOff = 1) AND (caqrr1.LevelRAG = 3) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (SelfAssessmentResultSupervisorVerifications_1.SignedOff = 1) AND (caqrr1.LevelRAG = 3) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (SelfAssessmentResultSupervisorVerifications_1.SignedOff = 1) AND (caqrr1.LevelRAG = 3) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS MeetingCount, 
             CASE WHEN COALESCE (casv.SignedOff, 0) = 1 THEN casv.Verified ELSE NULL END AS SignedOff, CASE WHEN COALESCE (casv.SignedOff, 0) = 1 THEN au.Forename + ' ' + au.Surname ELSE NULL END AS 'Signatory', CASE WHEN COALESCE (casv.SignedOff, 0) = 1 THEN (SELECT TOP(1) ProfessionalRegistrationNumber FROM Candidates as ca WHERE ca.EmailAddress = au.Email AND ca.CentreID = au.CentreID AND ca.Active = 1 AND ca.ProfessionalRegistrationNumber IS NOT NULL) ELSE NULL END AS SignatoryPrn
FROM   CandidateAssessmentSupervisorVerifications AS casv RIGHT OUTER JOIN
             CandidateAssessments AS ca INNER JOIN
             SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN
             Candidates AS c ON ca.CandidateID = c.CandidateID ON casv.ID =
                 (SELECT MAX(ID) AS ID
                 FROM    CandidateAssessmentSupervisorVerifications
                 WHERE (ID = ca.ID)) LEFT OUTER JOIN
             SupervisorDelegates AS sd LEFT OUTER JOIN
             AdminUsers as au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId ON casv.CandidateAssessmentSupervisorID = cas.ID
WHERE (ca.ID = @candidateAssessmentId  AND ca.CandidateID = @candidateId)",
                new { candidateAssessmentId, candidateId }
            );
        }

        public List<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId, int candidateId
        )
        {
            return connection.Query<CandidateAssessmentExportDetail>(
                @"WITH LatestAssessmentResults AS
            (
                SELECT
                    s.CompetencyID,
                    s.AssessmentQuestionID,
                    s.ID AS ResultID,
					COALESCE((SELECT LevelLabel FROM AssessmentQuestionLevels as aql WHERE aql.AssessmentQuestionID = s.AssessmentQuestionID AND aql.LevelValue = s.Result), CAST(s.Result AS nvarchar)) AS Result,
                    s.SupportingComments,
                    sv.ID AS SelfAssessmentResultSupervisorVerificationId,
                    au.Forename + ' ' + au.Surname AS Reviewer,
					COALESCE((SELECT TOP(1) ProfessionalRegistrationNumber FROM Candidates as ca WHERE ca.EmailAddress = au.Email AND ca.CentreID = au.CentreID AND ca.Active = 1 AND ca.ProfessionalRegistrationNumber IS NOT NULL), '') AS PRN,
                    sv.Verified,
                    sv.Comments,
                    sv.SignedOff,
                    0 AS UserIsVerifier,
                    COALESCE (rr.LevelRAG, 0) AS ResultRAG
                FROM CandidateAssessments ca
                INNER JOIN SelfAssessmentResults s
                    ON s.CandidateID = ca.CandidateID AND s.SelfAssessmentID = ca.SelfAssessmentID
                INNER JOIN (
                    SELECT MAX(s1.ID) as ID
                    FROM SelfAssessmentResults AS s1
                    INNER JOIN CandidateAssessments AS ca1
                        ON  s1.CandidateID = ca1.CandidateID AND s1.SelfAssessmentID = ca1.SelfAssessmentID
                    WHERE ca1.ID = @candidateAssessmentId
                    GROUP BY CompetencyID, AssessmentQuestionID
                ) t
                    ON s.ID = t.ID
                LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
                    ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
                LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas 
                    ON sv.CandidateAssessmentSupervisorID = cas.ID
                LEFT OUTER JOIN SupervisorDelegates AS sd
                    ON cas.SupervisorDelegateId = sd.ID
                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                    ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID
                        AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
               
				LEFT OUTER JOIN AdminUsers as au
					ON sd.SupervisorAdminID = au.AdminID
                 WHERE ca.ID = @candidateAssessmentId AND ca.CandidateID = @candidateId
            )
                    SELECT 
					CG.Name AS [Group],
            C.Name AS Competency,
            C.Description AS Description,
            SAS.Optional AS CompetencyOptional,
            AQ.Question AS AssessmentQuestion,
            CAQ.Required AS SelfAssessmentRequired,
            LAR.Result AS SelfAssessmentResult,
            LAR.SupportingComments AS SelfAssessmentComments,
            LAR.Reviewer,
			LAR.PRN AS ReviewerPrn,
            LAR.Verified AS Reviewed,
            LAR.Comments AS ReviewerComments,
            LAR.SignedOff AS ReviewerVerified,
			CASE WHEN LAR.Result IS NULL THEN 'Not self assessed'
			WHEN LAR.SignedOff = 0 THEN 'Self assessment not verified'
            WHEN LAR.ResultRAG = 0 THEN 'No requirements specified'
			WHEN LAR.ResultRAG = 1 THEN 'Not meeting requirements'
			WHEN LAR.ResultRAG = 2 THEN 'Partially meeting requirements'
			ELSE 'Meeting requirements' END AS RoleRequirements
                    FROM Competencies AS C
            INNER JOIN CompetencyAssessmentQuestions AS CAQ
                ON CAQ.CompetencyID = C.ID
            INNER JOIN AssessmentQuestions AS AQ
                ON AQ.ID = CAQ.AssessmentQuestionID
            INNER JOIN CandidateAssessments AS CA
                ON CA.ID = @candidateAssessmentId AND CA.CandidateID = @candidateId
            LEFT OUTER JOIN LatestAssessmentResults AS LAR
                ON LAR.CompetencyID = C.ID AND LAR.AssessmentQuestionID = AQ.ID
            INNER JOIN SelfAssessmentStructure AS SAS
                ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
            INNER JOIN CompetencyGroups AS CG
                ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
            LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID
                    WHERE (CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0)
                    ORDER BY SAS.Ordering, CAQ.Ordering",
                new { candidateAssessmentId, candidateId }
            ).AsList();
        }
    }
}
