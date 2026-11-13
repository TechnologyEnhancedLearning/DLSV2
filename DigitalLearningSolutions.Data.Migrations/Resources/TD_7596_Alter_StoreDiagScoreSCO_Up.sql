/****** Object:  StoredProcedure [dbo].[StoreDiagScoreSCO]    Script Date: 05/08/2026 08:51:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 08/08/2018
-- Description:	Updates the diagnostic score for a delegate using ProgressID and TutorialID to identify ASPProgress record to update
-- =============================================
ALTER PROCEDURE [dbo].[StoreDiagScoreSCO]
	@score INT,
	@progressid INT, 
	@TutorialID INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Step 1: Resolve the target TutorialID safely and efficiently
	DECLARE @TargetTutorialID INT;

	-- Get the ApplicationID associated with the given progress ID once
	SELECT TOP (1) @TargetTutorialID = t.TutorialID
	FROM Tutorials AS t 
	INNER JOIN Sections AS s ON t.SectionID = s.SectionID
	INNER JOIN Customisations AS c ON s.ApplicationID = c.ApplicationID
	INNER JOIN Progress AS p ON c.CustomisationID = p.CustomisationID
	WHERE p.ProgressID = @progressid 
	  AND (t.OriginalTutorialID = @TutorialID OR t.TutorialID = @TutorialID);

	-- Step 2: Perform the update on aspProgress
	UPDATE ap
	SET 
		DiagHigh = CASE WHEN ap.DiagHigh > @score THEN ap.DiagHigh ELSE @score END,
		 DiagLow = (CASE WHEN DiagLow < @score AND DiagAttempts > 0 THEN DiagLow ELSE @score END),
		DiagLast = @score,
		DiagAttempts = ap.DiagAttempts + 1
	FROM aspProgress AS ap
	WHERE ap.ProgressID = @progressid
	  AND ap.TutorialID = @TargetTutorialID;

END