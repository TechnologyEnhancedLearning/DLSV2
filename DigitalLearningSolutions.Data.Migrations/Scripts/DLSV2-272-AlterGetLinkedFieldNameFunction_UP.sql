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
	SELECT @ResultVar = CASE WHEN @LinkedFieldID = 0 THEN 'None' WHEN @LinkedFieldID = 4 THEN 'Job Group' WHEN @LinkedFieldID =1 THEN cp1.CustomPrompt WHEN @LinkedFieldID = 2 THEN cp2.CustomPrompt WHEN @LinkedFieldID = 3 THEN cp3.CustomPrompt
	WHEN @LinkedFieldID = 5 THEN cp4.CustomPrompt
	WHEN @LinkedFieldID = 6 THEN cp5.CustomPrompt
	WHEN @LinkedFieldID = 7 THEN cp6.CustomPrompt
	END 
	FROM   Centres AS c LEFT OUTER JOIN
             CustomPrompts AS cp1 ON c.CustomField1PromptID = cp1.CustomPromptID
LEFT OUTER JOIN
             CustomPrompts AS cp2 ON c.CustomField2PromptID = cp2.CustomPromptID
LEFT OUTER JOIN
             CustomPrompts AS cp3 ON c.CustomField3PromptID = cp3.CustomPromptID
LEFT OUTER JOIN
             CustomPrompts AS cp4 ON c.CustomField4PromptID = cp4.CustomPromptID
LEFT OUTER JOIN
             CustomPrompts AS cp5 ON c.CustomField5PromptID = cp5.CustomPromptID
LEFT OUTER JOIN
             CustomPrompts AS cp6 ON c.CustomField6PromptID = cp6.CustomPromptID
			 WHERE CentreID = @CentreID
	-- Return the result of the function
	RETURN @ResultVar

END

