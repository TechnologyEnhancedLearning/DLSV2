SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Auldrin Possa
-- Create date: 30/11/2023
-- Description:	Returns candidate assessment results by candidateAssessmentId
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetCandidateAssessmentResultsById]
	@candidateAssessmentId as Int = 0,
	@adminId as int = 0,
	@selfAssessmentResultId as int = NULL
AS
BEGIN

	SET NOCOUNT ON;

	WITH LatestAssessmentResults AS
            (
                SELECT
                    s.CompetencyID,
                    s.AssessmentQuestionID,
                    s.ID AS ResultID,
                    s.Result,
                    s.DateTime AS ResultDateTime,
                    s.SupportingComments,
                    sv.ID AS SelfAssessmentResultSupervisorVerificationId,
                    sv.Requested,
                    sv.Verified,
                    sv.Comments,
                    sv.SignedOff,
                    adu.Forename + ' ' + adu.Surname AS SupervisorName,
                    CAST(CASE WHEN COALESCE(sd.SupervisorAdminID, 0) = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsVerifier,
                    COALESCE (rr.LevelRAG, 0) AS ResultRAG
                FROM CandidateAssessments ca
                INNER JOIN SelfAssessmentResults s
                    ON s.DelegateUserID = ca.DelegateUserID AND s.SelfAssessmentID = ca.SelfAssessmentID
                LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
                    ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
                LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas 
                    ON sv.CandidateAssessmentSupervisorID = cas.ID AND cas.Removed IS NULL
                LEFT OUTER JOIN SupervisorDelegates AS sd
                    ON cas.SupervisorDelegateId = sd.ID
                LEFT OUTER JOIN AdminUsers AS adu
						     ON sd.SupervisorAdminID = adu.AdminID
                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                    ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID
                        AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
                WHERE ca.ID = @candidateAssessmentId
            )



		SELECT C.ID AS Id,
            DENSE_RANK() OVER (ORDER BY SAS.Ordering) as RowNo,
            C.Name AS Name,
            C.Description AS Description,
            CG.Name AS CompetencyGroup,
            CG.ID AS CompetencyGroupID,
            CG.Description AS CompetencyGroupDescription,
            COALESCE(
                (SELECT TOP(1) FrameworkConfig
                FROM Frameworks F
                INNER JOIN FrameworkCompetencies AS FC
                    ON FC.FrameworkID = F.ID
                WHERE FC.CompetencyID = C.ID),
            'Capability') AS Vocabulary,
            CASE
                WHEN (SELECT COUNT(*) FROM SelfAssessmentSupervisorRoles WHERE SelfAssessmentID = SAS.SelfAssessmentID) > 0
                THEN 1
                ELSE 0
            END AS HasDelegateNominatedRoles,
            SAS.Optional,
            C.AlwaysShowDescription,
            AQ.ID AS Id,
            AQ.Question,
            AQ.MaxValueDescription,
            AQ.MinValueDescription,
            AQ.ScoringInstructions,
            AQ.MinValue,
            AQ.MaxValue,
            AQ.AssessmentQuestionInputTypeID,
            AQ.IncludeComments,
            AQ.CommentsPrompt,
            AQ.CommentsHint,
            CAQ.Required,
            LAR.ResultId,
            LAR.Result,
            LAR.ResultDateTime,
            LAR.SupportingComments,
            LAR.SelfAssessmentResultSupervisorVerificationId,
            LAR.Requested,
            LAR.Verified,
            LAR.Comments AS SupervisorComments,
            LAR.SignedOff,
            LAR.UserIsVerifier,
            LAR.ResultRAG,
            LAR.SupervisorName

	FROM Competencies AS C
            INNER JOIN CompetencyAssessmentQuestions AS CAQ
                ON CAQ.CompetencyID = C.ID
            INNER JOIN AssessmentQuestions AS AQ
                ON AQ.ID = CAQ.AssessmentQuestionID
            INNER JOIN CandidateAssessments AS CA
                ON CA.ID = @candidateAssessmentId
            LEFT OUTER JOIN LatestAssessmentResults AS LAR
                ON LAR.CompetencyID = C.ID AND LAR.AssessmentQuestionID = AQ.ID
            INNER JOIN SelfAssessmentStructure AS SAS
                ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
            INNER JOIN CompetencyGroups AS CG
                ON SAS.CompetencyGroupID = CG.ID AND SAS.SelfAssessmentID = CA.SelfAssessmentID
            LEFT OUTER JOIN CandidateAssessmentOptionalCompetencies AS CAOC
                ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND CG.ID = CAOC.CompetencyGroupID


	WHERE (CAOC.IncludedInSelfAssessment = 1 OR SAS.Optional = 0)
			AND (@selfAssessmentResultId IS NULL OR 
			(@selfAssessmentResultId IS NOT NULL AND ResultID = @selfAssessmentResultId))

	ORDER BY SAS.Ordering, CAQ.Ordering
END
GO
