/****** Object:  StoredProcedure [dbo].[GetFilteredCompetencyResponsesForCandidate]    Script Date: 22/09/2020 09:22:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 22/09/2020
-- Description:	Returns user self assessment responses (AVG) for Filtered competency
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetFilteredCompetencyResponsesForCandidate]
	-- Add the parameters for the stored procedure here
	@SelfAssessmentID int,
	@CandidateID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT FilteredCompetencyID AS id, Case WHEN [2] = 0.0 THEN 0.1 WHEN [2] = 1.0 THEN 0.9 ELSE [2] END AS importance, Case WHEN [1] = 0.0 THEN 0.1 WHEN [1] = 1.0 THEN 0.9 ELSE [1] END AS confidence
FROM   (SELECT fcm.FilteredCompetencyID, sar.AssessmentQuestionID, sar.Result * 0.1 AS Result
FROM   FilteredComptenencyMapping AS fcm INNER JOIN
             Competencies AS com ON fcm.CompetencyID = com.ID INNER JOIN
             SelfAssessmentResults AS sar ON com.ID = sar.CompetencyID
             WHERE  (sar.SelfAssessmentID = @SelfAssessmentID) AND (sar.CandidateID = @CandidateID)) sr PIVOT (AVG(Result) FOR AssessmentQuestionID IN ([1], [2])) AS pvt
END
GO