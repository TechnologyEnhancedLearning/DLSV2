/****** Object:  StoredProcedure [dbo].[uspReturnSectionsForCandCust_V2]    Script Date: 08/12/2023 13:33:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 15/08/2013
-- Description:	Gets section table for learning menu
-- =============================================
ALTER PROCEDURE [dbo].[uspReturnSectionsForCandCust_V2]
	-- Add the parameters for the stored procedure here
	@ProgressID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	UPDATE [dbo].[Progress]
   SET [LoginCount] =  (SELECT COALESCE(COUNT(*), 0)
                                                            FROM      Sessions AS s
                                                            WHERE   (CandidateID = [Progress].CandidateID) AND (CustomisationID = [Progress].CustomisationID) AND (LoginTime BETWEEN [Progress].FirstSubmittedTime AND [Progress].SubmittedTime))
      ,[Duration] = (SELECT COALESCE(SUM(Duration), 0)
                                                            FROM      Sessions AS s1
                                                            WHERE   (CandidateID = [Progress].CandidateID) AND (CustomisationID = [Progress].CustomisationID) AND (LoginTime BETWEEN [Progress].FirstSubmittedTime AND [Progress].SubmittedTime))
															WHERE ProgressID = @ProgressID



SELECT        S.SectionID, S.ApplicationID, S.SectionNumber, S.SectionName, (SUM(asp1.TutStat) * 100) / (COUNT(T.TutorialID) * 2) AS PCComplete, SUM(asp1.TutTime) AS TimeMins, MAX(ISNULL(asp1.DiagAttempts, 0)) AS DiagAttempts, 
                         SUM(asp1.DiagLast) AS SecScore, SUM(T.DiagAssessOutOf) AS SecOutOf, S.ConsolidationPath, S.AverageSectionMins AS AvgSecTime, S.DiagAssessPath, S.PLAssessPath, MAX(ISNULL(CAST(CT.Status AS Integer), 0)) 
                         AS LearnStatus, MAX(ISNULL(CAST(CT.DiagStatus AS Integer), 0)) AS DiagStatus, COALESCE (MAX(ISNULL(aa.Score, 0)), 0) AS MaxScorePL,
                             (SELECT        COUNT(AssessAttemptID) AS PLAttempts
                               FROM            AssessAttempts AS aa
                               WHERE        (ProgressID = @ProgressID) AND (SectionNumber = S.SectionNumber)) AS AttemptsPL, COALESCE (MAX(ISNULL(CAST(aa.Status AS Integer), 0)), 0) AS PLPassed, 
                         cu.IsAssessed, p.PLLocked, dbo.CheckCustomisationSectionHasLearning(p.CustomisationID, S.SectionID) AS HasLearning
FROM            aspProgress AS asp1 INNER JOIN
                         Progress AS p ON asp1.ProgressID = p.ProgressID INNER JOIN
                         Sections AS S INNER JOIN
                         Tutorials AS T ON S.SectionID = T.SectionID INNER JOIN
                         CustomisationTutorials AS CT ON T.TutorialID = CT.TutorialID ON asp1.TutorialID = T.TutorialID INNER JOIN
                         Customisations AS cu ON p.CustomisationID = cu.CustomisationID LEFT OUTER JOIN
                         AssessAttempts AS aa ON p.ProgressID = aa.ProgressID AND S.SectionNumber = aa.SectionNumber
WHERE         (CT.CustomisationID = p.CustomisationID) AND (p.ProgressID = @ProgressID) AND (CT.Status = 1) AND (S.ArchivedDate IS NULL) OR
                         (CT.CustomisationID = p.CustomisationID) AND (p.ProgressID = @ProgressID)  AND (S.ArchivedDate IS NULL) AND (CT.DiagStatus = 1) OR
                         (CT.CustomisationID = p.CustomisationID) AND (p.ProgressID = @ProgressID)  AND (S.ArchivedDate IS NULL) AND (cu.IsAssessed = 1)
GROUP BY S.SectionID, S.ApplicationID, S.SectionNumber, S.SectionName, S.ConsolidationPath, S.DiagAssessPath, S.PLAssessPath, S.AverageSectionMins, cu.IsAssessed, p.CandidateID, p.CustomisationID, p.PLLocked
ORDER BY S.SectionNumber
END


GO
