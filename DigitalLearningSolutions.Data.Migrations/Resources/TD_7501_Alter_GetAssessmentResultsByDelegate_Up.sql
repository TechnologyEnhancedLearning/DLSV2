
/****** Object:  StoredProcedure [dbo].[GetAssessmentResultsByDelegate]    Script Date: 28/07/2026 15:06:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Auldrin Possa
-- Create date: 30/11/2023
-- Description:	Returns assessment results for a delegate
-- =============================================
ALTER   PROCEDURE [dbo].[GetAssessmentResultsByDelegate]
	@selfAssessmentId as Int = 0,
	@delegateId as int = 0
AS
BEGIN

	SET NOCOUNT ON;

	WITH LatestAssessmentResults AS
            (
                SELECT
                    s.CompetencyID,
                    s.AssessmentQuestionID,
                    s.ID AS ResultID,
                    s.DateTime AS ResultDateTime,
                    s.Result,
                    s.SupportingComments,
                    sv.ID AS SelfAssessmentResultSupervisorVerificationId,
                    sv.Requested,
                    sv.Verified,
                    sv.Comments,
                    sv.SignedOff,
                    adu.Forename + ' ' + adu.Surname AS SupervisorName,
                    sv.CandidateAssessmentSupervisorID,
                    sv.EmailSent,
                    0 AS UserIsVerifier,
                    COALESCE (rr.LevelRAG, 0) AS ResultRAG
                FROM SelfAssessmentResults s
                LEFT OUTER JOIN DelegateAccounts AS da ON s.DelegateUserID = da.UserID
                LEFT OUTER JOIN SelfAssessmentResultSupervisorVerifications AS sv
                    ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0
                LEFT OUTER JOIN CandidateAssessmentSupervisors AS cas 
                    ON sv.CandidateAssessmentSupervisorID = cas.ID
                LEFT OUTER JOIN SupervisorDelegates AS sd
                    ON cas.SupervisorDelegateId = sd.ID
                LEFT OUTER JOIN AdminUsers AS adu
					ON sd.SupervisorAdminID = adu.AdminID
                LEFT OUTER JOIN CompetencyAssessmentQuestionRoleRequirements rr
                    ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID
                        AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
                WHERE da.ID = @delegateId
            )


		SELECT C.ID AS Id,
            DENSE_RANK() OVER (ORDER BY SAS.Ordering) as RowNo,
            C.Name AS Name,
            C.Description AS Description,
            COALESCE(CG.Name, 'Ungrouped') AS CompetencyGroup,
		    COALESCE(CG.ID, 0) AS CompetencyGroupID,
			COALESCE(CG.Description, 'Competencies without a group') AS CompetencyGroupDescription,
            COALESCE(
                (SELECT TOP(1) FrameworkConfig
                FROM Frameworks F
                INNER JOIN FrameworkCompetencies AS FC
                    ON FC.FrameworkID = F.ID
                WHERE FC.CompetencyID = C.ID),
            'Capability') AS Vocabulary,
            1 AS HasDelegateNominatedRoles,
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
                ON CA.SelfAssessmentID = @selfAssessmentId AND CA.RemovedDate IS NULL
			INNER JOIN DelegateAccounts AS DA ON CA.DelegateUserID = DA.UserID AND DA.ID = @delegateId
            LEFT OUTER JOIN LatestAssessmentResults AS LAR
                ON LAR.CompetencyID = C.ID AND LAR.AssessmentQuestionID = AQ.ID
            INNER JOIN SelfAssessmentStructure AS SAS
                ON C.ID = SAS.CompetencyID AND SAS.SelfAssessmentID = @selfAssessmentId
           LEFT OUTER JOIN CompetencyGroups AS CG
                ON  SAS.CompetencyGroupID = CG.ID  AND SAS.SelfAssessmentID = @selfAssessmentId 
           LEFT OUTER JOIN (
          SELECT
          CandidateAssessmentID,
          CompetencyID,
		CompetencyGroupID,
        CAST(MAX(CAST(IncludedInSelfAssessment AS int)) AS bit) AS IncludedInSelfAssessment
       FROM CandidateAssessmentOptionalCompetencies
       GROUP BY CandidateAssessmentID, CompetencyID, CompetencyGroupID
      ) AS CAOC
               ON CA.ID = CAOC.CandidateAssessmentID AND C.ID = CAOC.CompetencyID AND (
        CG.ID = CAOC.CompetencyGroupID
        OR (CG.ID IS NULL AND CAOC.CompetencyGroupID IS NULL)
       )

		WHERE (CAOC.IncludedInSelfAssessment = 1) OR (SAS.Optional = 0)
		ORDER BY SAS.Ordering, CAQ.Ordering
END
