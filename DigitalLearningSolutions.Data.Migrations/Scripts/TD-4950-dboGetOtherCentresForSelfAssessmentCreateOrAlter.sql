/****** Object:  UserDefinedFunction [dbo].[GetOtherCentresForSelfAssessment]    Script Date: 12/11/2024 08:47:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 22/01/2024
-- Description:	Gets a comma separated list of other centres for a user self assessment
-- =============================================
CREATE OR ALTER FUNCTION [dbo].[GetOtherCentresForSelfAssessment]
(
	-- Add the parameters for the function here
	 @userId int,
	 @selfAssessmentId int,
	 @excludeCentreId int
)
RETURNS nvarchar(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar nvarchar(MAX);
	-- Add the T-SQL statements to compute the return value here
SELECT @ResultVar = COALESCE(@ResultVar + ', ','')+ Q1.CentreName
	FROM  (SELECT DISTINCT c.CentreName
FROM   Users AS u INNER JOIN
             DelegateAccounts AS da ON u.ID = da.UserID INNER JOIN
             Centres AS c ON da.CentreID = c.CentreID INNER JOIN
             CentreSelfAssessments AS csa ON c.CentreID = csa.CentreID
WHERE (u.ID = @userId) AND (da.Active = 1) AND (da.Approved = 1) AND (csa.SelfAssessmentID = @selfAssessmentId) AND (c.CentreID <> @excludeCentreId)) AS Q1
	-- Return the result of the function
	RETURN @ResultVar

END
GO


