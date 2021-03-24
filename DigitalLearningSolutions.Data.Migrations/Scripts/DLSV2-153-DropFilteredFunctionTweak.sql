
/****** Object:  UserDefinedFunction [dbo].[GetSelfAssessmentSummaryForCandidate]    Script Date: 28/01/2021 07:45:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[GetSelfAssessmentSummaryForCandidate]
(
	@CandidateID int,
	@SelfAssessmentID int
)
RETURNS @ResTable TABLE 
(
	CompetencyGroupID int,
	Confidence float,
	Relevance float
)

AS	  
BEGIN
INSERT INTO @ResTable
	SELECT CompetencyGroupID, [1] AS Confidence, [2] AS Relevance
FROM   (SELECT comp.CompetencyGroupID, sar.AssessmentQuestionID, sar.Result*1.0 AS Result
             FROM    Competencies AS comp INNER JOIN
                           SelfAssessmentResults AS sar ON comp.ID = sar.CompetencyID
             WHERE  (sar.SelfAssessmentID = @SelfAssessmentID) AND (sar.CandidateID = @CandidateID)) sr PIVOT (AVG(Result) FOR AssessmentQuestionID IN ([1], [2])) AS pvt
RETURN
END
GO


