
/****** Object:  StoredProcedure [dbo].[GetCompletedCoursesForCandidate]    Script Date: 20/08/2024 11:58:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 16/12/2016
-- Description:	Returns a list of completed courses for the candidate.
-- 21/06/2021: Adds Applications.ArchivedDate field to output.
-- =============================================
ALTER PROCEDURE [dbo].[GetCompletedCoursesForCandidate]
	-- Add the parameters for the stored procedure here
	@CandidateID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT p.ProgressID, (CASE WHEN cu.CustomisationName <> '' THEN a.ApplicationName + ' - ' + cu.CustomisationName ELSE a.ApplicationName END) AS CourseName, p.CustomisationID, p.SubmittedTime AS LastAccessed, p.Completed, 
               p.FirstSubmittedTime AS StartedDate, p.RemovedDate, p.DiagnosticScore, p.PLLocked, cu.IsAssessed, dbo.CheckCustomisationSectionHasDiagnostic(p.CustomisationID, 0) 
               AS HasDiagnostic, dbo.CheckCustomisationSectionHasLearning(p.CustomisationID, 0) AS HasLearning,
                     COALESCE
                             ((SELECT        COUNT(Passes) AS Passes
                                 FROM            (SELECT        COUNT(AssessAttemptID) AS Passes
                                                           FROM            AssessAttempts AS aa
                                                           WHERE        (CandidateID = p.CandidateID) AND (CustomisationID = p.CustomisationID) AND (Status = 1)
                                                           GROUP BY SectionNumber) AS derivedtbl_2), 0) AS Passes,
                   (SELECT COUNT(SectionID) AS Sections
                    FROM   Sections AS s
                    WHERE (ApplicationID = cu.ApplicationID)) AS Sections, p.Evaluated, p.FollupUpEvaluated, a.ArchivedDate
FROM  Progress AS p INNER JOIN
               Customisations AS cu ON p.CustomisationID = cu.CustomisationID INNER JOIN
               Applications AS a ON cu.ApplicationID = a.ApplicationID INNER JOIN 
		       CentreApplications AS ca ON ca.ApplicationID = a.ApplicationID AND ca.CentreID = cu.CentreID
WHERE (NOT (p.Completed IS NULL)) AND (p.CandidateID = @CandidateID)
ORDER BY p.Completed DESC
	
END
GO



