/****** Object:  StoredProcedure [dbo].[GetCompletedCoursesForCandidate]    Script Date: 06/08/2026 09:15:39 ******/
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
    @CandidateID INT
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
    SET NOCOUNT ON;
    -- Insert statements for procedure here
    SELECT 
        p.ProgressID,
        CASE 
            WHEN ISNULL(cu.CustomisationName, '') <> '' THEN a.ApplicationName + ' - ' + cu.CustomisationName 
            ELSE a.ApplicationName 
        END AS CourseName,
        p.CustomisationID,
        cu.Active,
        p.SubmittedTime AS LastAccessed,
        p.Completed, 
        p.FirstSubmittedTime AS StartedDate,
        p.RemovedDate,
        p.DiagnosticScore,
        p.PLLocked,
        cu.IsAssessed,
        dbo.CheckCustomisationSectionHasDiagnostic(p.CustomisationID, 0) AS HasDiagnostic, 
        dbo.CheckCustomisationSectionHasLearning(p.CustomisationID, 0) AS HasLearning,
        
        -- Safe & simplified calculation for unique passed sections
        ISNULL(pass.PassesCount, 0) AS Passes,
        sec.SectionCount AS Sections,
        
        p.Evaluated,
        p.FollupUpEvaluated,
        a.ArchivedDate,
        
        -- Standardized EXISTS check for unpublished course status
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM CentreApplications ca 
                WHERE ca.ApplicationID = a.ApplicationID 
                  AND ca.CentreID = cu.CentreID
            ) THEN 1 
            ELSE 0 
        END AS CheckUnpublishedCourse

    FROM Progress AS p WITH (NOLOCK)
    INNER JOIN Customisations AS cu WITH (NOLOCK) 
        ON p.CustomisationID = cu.CustomisationID
    INNER JOIN Applications AS a WITH (NOLOCK) 
        ON cu.ApplicationID = a.ApplicationID

    -- Calculate unique passed sections per Customisation & Candidate
    OUTER APPLY (
        SELECT COUNT(DISTINCT aa.SectionNumber) AS PassesCount
        FROM AssessAttempts AS aa WITH (NOLOCK)
        WHERE aa.CandidateID = p.CandidateID
          AND aa.CustomisationID = p.CustomisationID
          AND aa.Status = 1
    ) AS pass

    -- Calculate total sections per Application
    OUTER APPLY (
        SELECT COUNT(s.SectionID) AS SectionCount
        FROM Sections AS s WITH (NOLOCK)
        WHERE s.ApplicationID = cu.ApplicationID
    ) AS sec

    WHERE p.CandidateID = @CandidateID
      AND p.Completed IS NOT NULL

    ORDER BY p.Completed DESC;
END;