/****** Object:  UserDefinedFunction [dbo].[CheckDelegateStatusForCustomisation]    Script Date: 16/05/2024 09:29:31 ******/
SET ANSI_NULLS ON
GO
 
SET QUOTED_IDENTIFIER ON
GO
 
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 22/12/2016
-- Description:	Checks if learner has progress record against customisation.
-- Returns:
-- 0: None
-- 1: Expired
-- 2: Complete
-- 3: Current
-- =============================================
-- 18/09/2018 Adds return val of 4 for removed progress. Also excluded removed progress from current and expired checks.
-- 17/20/2023 Excludes progress that is marked as removed from the query with a return value of 2 (Complete).
ALTER FUNCTION [dbo].[CheckDelegateStatusForCustomisation]
(
	-- Add the parameters for the function here
	@CustomisationID Int,
	@CandidateID Int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar int
	Set @ResultVar = 0
 
	-- Add the T-SQL statements to compute the return value here
	-- Check of current:
	if @CustomisationID IN (SELECT CustomisationID
FROM  Progress AS p
WHERE (Completed IS NULL) AND (RemovedDate IS NULL) AND (CandidateID = @CandidateID) AND (SubmittedTime > DATEADD(M, - 6, GETDATE())  OR NOT p.CompleteByDate IS NULL))
begin
Set @ResultVar = 3
goto onExit
end
	--Check if Complete:
	if @CustomisationID IN (SELECT CustomisationID
FROM  Progress AS p
WHERE (Completed IS NOT NULL) AND (RemovedDate IS NULL) AND (CandidateID = @CandidateID))
begin
Set @ResultVar = 2
goto onExit
end
--Check if Expired:
if @CustomisationID IN (SELECT CustomisationID
FROM  Progress AS p
WHERE (Completed IS NULL) AND (RemovedDate IS NULL) AND (CandidateID = @CandidateID) AND (SubmittedTime <= DATEADD(M, - 6, GETDATE())) AND (p.CompleteByDate IS NULL))
begin
Set @ResultVar = 1
goto onExit
end
	--Check if Removed:
	if @CustomisationID IN (SELECT CustomisationID
FROM  Progress AS p
WHERE (Completed IS NULL) AND (RemovedDate IS NOT NULL) AND (CandidateID = @CandidateID) AND (p.CompleteByDate IS NULL))
begin
Set @ResultVar = 4
goto onExit
end
	-- Return the result of the function
	onExit:
	RETURN @ResultVar
 
END
 
GO
 