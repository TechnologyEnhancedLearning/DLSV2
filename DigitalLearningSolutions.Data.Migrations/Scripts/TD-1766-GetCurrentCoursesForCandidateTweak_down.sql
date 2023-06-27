/****** Object:  StoredProcedure [dbo].[GetCurrentCoursesForCandidate_V2]    Script Date: 22/06/2023 14:49:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 16/12/2016
-- Description:	Returns a list of active progress records for the candidate.
-- Change 18/09/2018: Adds logic to exclude Removed courses from returned results.
-- =============================================
ALTER PROCEDURE [dbo].[GetCurrentCoursesForCandidate_V2]
	-- Add the parameters for the stored procedure here
	@CandidateID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT p.ProgressID, a.ApplicationName + ' - ' + cu.CustomisationName AS CourseName, p.CustomisationID, p.SubmittedTime AS LastAccessed, 
               p.FirstSubmittedTime AS StartedDate, p.DiagnosticScore, p.PLLocked, cu.IsAssessed, dbo.CheckCustomisationSectionHasDiagnostic(p.CustomisationID, 0) 
               AS HasDiagnostic, dbo.CheckCustomisationSectionHasLearning(p.CustomisationID, 0) AS HasLearning,
                   COALESCE
                             ((SELECT        COUNT(Passes) AS Passes
                                 FROM            (SELECT        COUNT(AssessAttemptID) AS Passes
                                                           FROM            AssessAttempts AS aa
                                                           WHERE        (CandidateID = p.CandidateID) AND (CustomisationID = p.CustomisationID) AND (Status = 1)
                                                           GROUP BY SectionNumber) AS derivedtbl_2), 0) AS Passes,
                   (SELECT COUNT(SectionID) AS Sections
                    FROM   Sections AS s
                    WHERE (ApplicationID = cu.ApplicationID)) AS Sections, p.CompleteByDate, CAST(CASE WHEN p.CompleteByDate IS NULL THEN 0 WHEN p.CompleteByDate < getDate() 
                         THEN 2 WHEN p.CompleteByDate < DATEADD(M, + 1, getDate()) THEN 1 ELSE 0 END AS INT) AS OverDue, p.EnrollmentMethodID, dbo.GetCohortGroupCustomisationID(p.ProgressID) AS GroupCustomisationID, p.SupervisorAdminID

FROM  Progress AS p INNER JOIN
               Customisations AS cu ON p.CustomisationID = cu.CustomisationID INNER JOIN
               Applications AS a ON cu.ApplicationID = a.ApplicationID
WHERE (p.Completed IS NULL) AND (p.RemovedDate IS NULL) AND (p.CandidateID = @CandidateID)AND (cu.CustomisationName <> 'ESR') AND (a.ArchivedDate IS NULL) AND (cu.Active = 1) AND (p.SubmittedTime > DATEADD(M, -6, getDate()) OR NOT p.CompleteByDate IS NULL)
ORDER BY p.SubmittedTime Desc
END

GO
