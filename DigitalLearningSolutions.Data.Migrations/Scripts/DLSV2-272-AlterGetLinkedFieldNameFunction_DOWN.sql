/****** Object:  UserDefinedFunction [dbo].[GetLinkedFieldNameForCentreByID]    Script Date: 16/07/2021 09:23:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 12/10/2018
-- Description:	Return the field name for centre group linked field ID
-- =============================================
ALTER FUNCTION [dbo].[GetLinkedFieldNameForCentreByID] 
(
	-- Add the parameters for the function here
	@LinkedFieldID int,
	@CentreID int
)
RETURNS nvarchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar nvarchar(100)
	SET @ResultVar = ''
	-- Add the T-SQL statements to compute the return value here
	SELECT @ResultVar = CASE WHEN @LinkedFieldID = 0 THEN 'None' WHEN @LinkedFieldID = 4 THEN 'Job Group' WHEN @LinkedFieldID =1 THEN F1Name WHEN @LinkedFieldID = 2 THEN F2Name WHEN @LinkedFieldID = 3 THEN F3Name END FROM Centres WHERE CentreID = @CentreID
	-- Return the result of the function
	RETURN @ResultVar

END

