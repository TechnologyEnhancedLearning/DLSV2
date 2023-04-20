namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System.Collections.Generic;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;

    public partial class SelfAssessmentDataService
    {
        public CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(int candidateAssessmentId, int delegateUserID)
        {
            return connection.QuerySingle<CandidateAssessmentExportSummary>(
                @"SELECT sa.Name AS SelfAssessment, u.FirstName + ' ' + u.LastName AS CandidateName, u.ProfessionalRegistrationNumber AS CandidatePrn, ca.StartedDate AS StartDate,
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
                              SelfAssessmentResults AS sar1 ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS SelfAssessmentResponseCount,
                 (SELECT COUNT(sas1.CompetencyID) AS VerifiedCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS sarsv INNER JOIN
                              SelfAssessmentResults AS sar1 ON sarsv.SelfAssessmentResultId = sar1.ID AND sarsv.Superceded = 0 RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (sarsv.SignedOff = 1) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (sarsv.SignedOff = 1) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS ResponsesVerifiedCount,
                 (SELECT COUNT(sas1.CompetencyID) AS NoRequirementsSetCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS sarsv INNER JOIN
                              SelfAssessmentResults AS sar1 ON sarsv.SelfAssessmentResultId = sar1.ID AND sarsv.Superceded = 0 LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.ID IS NULL) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.ID IS NULL) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (sarsv.SignedOff = 1) AND (caqrr1.ID IS NULL) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (sarsv.SignedOff = 1) AND (caqrr1.ID IS NULL) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS NoRequirementsSetCount,
                 (SELECT COUNT(sas1.CompetencyID) AS NotMeetingCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS sarsv INNER JOIN
                              SelfAssessmentResults AS sar1 ON sarsv.SelfAssessmentResultId = sar1.ID AND sarsv.Superceded = 0 LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 1) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS NotMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS PartiallyMeeting
                 FROM    SelfAssessmentResultSupervisorVerifications AS sarsv INNER JOIN
                              SelfAssessmentResults AS sar1 ON sarsv.SelfAssessmentResultId = sar1.ID AND sarsv.Superceded = 0 LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 2) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS PartiallyMeetingCount,
                 (SELECT COUNT(sas1.CompetencyID) AS MeetingCount
                 FROM    SelfAssessmentResultSupervisorVerifications AS sarsv INNER JOIN
                              SelfAssessmentResults AS sar1 ON sarsv.SelfAssessmentResultId = sar1.ID AND sarsv.Superceded = 0 LEFT OUTER JOIN
                              CompetencyAssessmentQuestionRoleRequirements AS caqrr1 ON sar1.Result = caqrr1.LevelValue AND sar1.CompetencyID = caqrr1.CompetencyID AND sar1.SelfAssessmentID = caqrr1.SelfAssessmentID AND 
                              sar1.AssessmentQuestionID = caqrr1.AssessmentQuestionID RIGHT OUTER JOIN
                              SelfAssessmentStructure AS sas1 INNER JOIN
                              CandidateAssessments AS ca1 ON sas1.SelfAssessmentID = ca1.SelfAssessmentID INNER JOIN
                              CompetencyAssessmentQuestions AS caq1 ON sas1.CompetencyID = caq1.CompetencyID ON sar1.SelfAssessmentID =sas1.SelfAssessmentID and sar1.CompetencyID=sas1.CompetencyID AND sar1.AssessmentQuestionID = caq1.AssessmentQuestionID AND sar1.DelegateUserID = ca1.DelegateUserID LEFT OUTER JOIN
                              CandidateAssessmentOptionalCompetencies AS caoc1 ON sas1.CompetencyID = caoc1.CompetencyID AND sas1.CompetencyGroupID = caoc1.CompetencyGroupID AND ca1.ID = caoc1.CandidateAssessmentID
                 WHERE (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) OR
                              (ca1.ID = ca.ID) AND (NOT (sar1.Result IS NULL)) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) AND (caoc1.IncludedInSelfAssessment = 1) OR
                              (ca1.ID = ca.ID) AND (sas1.Optional = 0) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) AND (NOT (sar1.SupportingComments IS NULL)) OR
                              (ca1.ID = ca.ID) AND (sarsv.SignedOff = 1) AND (caqrr1.LevelRAG = 3) AND (caoc1.IncludedInSelfAssessment = 1) AND (NOT (sar1.SupportingComments IS NULL))) AS MeetingCount, 
             CASE WHEN COALESCE (casv.SignedOff, 0) = 1 THEN casv.Verified ELSE NULL END AS SignedOff, CASE WHEN COALESCE (casv.SignedOff, 0) = 1 THEN au.Forename + ' ' + au.Surname + ' (' + au.CentreName + ')' ELSE NULL END AS 'Signatory', CASE WHEN COALESCE (casv.SignedOff, 0) = 1 THEN (SELECT TOP(1) ProfessionalRegistrationNumber FROM Candidates as ca WHERE ca.EmailAddress = au.Email AND ca.CentreID = au.CentreID AND ca.Active = 1 AND ca.ProfessionalRegistrationNumber IS NOT NULL) ELSE NULL END AS SignatoryPrn
FROM   CandidateAssessmentSupervisorVerifications AS casv RIGHT OUTER JOIN
             CandidateAssessments AS ca INNER JOIN
             SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID INNER JOIN Users AS u
			 ON u.ID = ca.DelegateUserID ON casv.ID =
                 (SELECT MAX(casv1.ID) AS ID
                 FROM    CandidateAssessmentSupervisorVerifications as casv1 INNER JOIN CandidateAssessmentSupervisors as cas1 ON casv1.CandidateAssessmentSupervisorID = cas1.ID
                 WHERE (cas1.CandidateAssessmentID = ca.ID)) LEFT OUTER JOIN
             SupervisorDelegates AS sd LEFT OUTER JOIN
             AdminUsers as au ON sd.SupervisorAdminID = au.AdminID LEFT OUTER JOIN
             CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId ON casv.CandidateAssessmentSupervisorID = cas.ID
WHERE (ca.ID = @candidateAssessmentId  AND ca.DelegateUserID  = @delegateUserID)",
                new { candidateAssessmentId, delegateUserID }
            );
        }

        public List<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId, int delegateUserID
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
                    au.Forename + ' ' + au.Surname+ ' (' + au.CentreName + ')' AS Reviewer,
					COALESCE((SELECT TOP(1) ProfessionalRegistrationNumber FROM Candidates as ca WHERE ca.EmailAddress = au.Email AND ca.CentreID = au.CentreID AND ca.Active = 1 AND ca.ProfessionalRegistrationNumber IS NOT NULL), '') AS PRN,
                    sv.Verified,
                    sv.Comments,
                    sv.SignedOff,
                    0 AS UserIsVerifier,
                    COALESCE (rr.LevelRAG, 0) AS ResultRAG
                FROM CandidateAssessments ca
                INNER JOIN SelfAssessmentResults s
                    ON s.DelegateUserID = ca.DelegateUserID AND s.SelfAssessmentID = ca.SelfAssessmentID           
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
                 WHERE ca.ID = @candidateAssessmentId AND ca.DelegateUserID = @delegateUserID
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
        	CASE WHEN (LAR.Reviewer = '') THEN ''
                 WHEN ((LAR.PRN IS NULL OR LAR.PRN ='') AND LAR.Reviewer !='') THEN 'Not Recorded'
                 WHEN ((LAR.PRN IS NOT NULL) AND LAR.Reviewer !='') THEN 'Recorded'
			ELSE '' END AS ReviewerPrn,
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
                ON CA.ID = @candidateAssessmentId AND CA.DelegateUserID = @delegateUserID
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
                new { candidateAssessmentId, delegateUserID }
            ).AsList();
        }
    }
}
