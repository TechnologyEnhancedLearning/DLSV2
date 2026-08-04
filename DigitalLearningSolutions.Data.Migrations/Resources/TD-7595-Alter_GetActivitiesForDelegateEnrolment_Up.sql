/****** Object:  StoredProcedure [dbo].[GetActivitiesForDelegateEnrolment]    Script Date: 04/08/2026 12:55:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 24/01/2023
-- Description:	Returns active available for delegate enrolment based on original GetActiveAvailableCustomisationsForCentreFiltered_V6 sproc but adjusted for user account refactor and filters properly for category.
-- =============================================
ALTER PROCEDURE [dbo].[GetActivitiesForDelegateEnrolment]
	@CentreID INT = 0,
	@DelegateID INT,
	@CategoryId INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Pre-fetch variables and current date to keep expressions sargable
	DECLARE @DelegateUserID INT;
	SELECT @DelegateUserID = UserID FROM dbo.DelegateAccounts WHERE ID = @DelegateID;

	DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);

	WITH CustomisationData AS (
		SELECT 
			cu.CustomisationID, 
			cu.Active, 
			cu.CurrentVersion, 
			cu.CentreID, 
			cu.ApplicationID, 
			CASE 
				WHEN cu.CustomisationName <> '' THEN a.ApplicationName + ' - ' + cu.CustomisationName 
				ELSE a.ApplicationName 
			END AS CourseName, 
			cu.CustomisationText, 
			0 AS IncludesSignposting, 
			0 AS IsSelfAssessment, 
			cu.SelfRegister AS SelfRegister,
			cu.IsAssessed, 
			dbo.CheckCustomisationSectionHasDiagnostic(cu.CustomisationID, 0) AS HasDiagnostic, 
			dbo.CheckCustomisationSectionHasLearning(cu.CustomisationID, 0) AS HasLearning, 
			b.BrandName AS Brand, 
			a.CourseCategoryID AS CategoryID,
			cc.CategoryName AS Category,
			ct.CourseTopic AS Topic, 
			ds.DelegateStatus,
			cu.HideInLearnerPortal
		FROM dbo.Customisations AS cu
		INNER JOIN dbo.Applications AS a 
			ON cu.ApplicationID = a.ApplicationID
		INNER JOIN dbo.CentreApplications AS ca 
			ON ca.ApplicationID = a.ApplicationID AND ca.CentreID = cu.CentreID
		LEFT JOIN dbo.Brands AS b 
			ON b.BrandID = a.BrandID
		LEFT JOIN dbo.CourseCategories AS cc 
			ON cc.CourseCategoryID = a.CourseCategoryID
		LEFT JOIN dbo.CourseTopics AS ct 
			ON ct.CourseTopicID = a.CourseTopicID
		-- Evaluate UDF once per row via CROSS APPLY
		CROSS APPLY (
			SELECT dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @DelegateID) AS DelegateStatus
		) AS ds
		WHERE cu.Active = 1
			AND a.ASPMenu = 1
			AND a.ArchivedDate IS NULL
			AND cu.CustomisationName <> 'ESR'
			AND ds.DelegateStatus IN (0, 1, 4)
			AND (@CategoryId = 0 OR a.CourseCategoryID = @CategoryId)
			AND (
				cu.CentreID = @CentreID 
				OR (
					cu.AllCentres = 1 
					AND EXISTS (
						SELECT 1 
						FROM dbo.CentreApplications 
						WHERE ApplicationID = a.ApplicationID 
							AND CentreID = @CentreID 
							AND Active = 1
					)
				)
			)

		UNION ALL

		SELECT 
			SA.ID AS CustomisationID, 
			1 AS Active, 
			1 AS CurrentVersion, 
			CSA.CentreID AS CentreID, 
			0 AS ApplicationID, 
			SA.Name AS CourseName, 
			SA.Description AS CustomisationText, 
			SA.IncludesSignposting, 
			1 AS IsSelfAssessment, 
			CSA.AllowEnrolment AS SelfRegister, 
			0 AS IsAssessed, 
			0 AS HasDiagnostic, 
			0 AS HasLearning,
			b.BrandName AS Brand,
			SA.CategoryID AS CategoryID,
			cc.CategoryName AS Category,
			'' AS Topic, 
			IIF(CA.RemovedDate IS NULL, 0, 1) AS DelegateStatus,
			0 AS HideInLearnerPortal
		FROM dbo.SelfAssessments AS SA 
		INNER JOIN dbo.CentreSelfAssessments AS CSA 
			ON SA.ID = CSA.SelfAssessmentID AND CSA.CentreID = @CentreID
		LEFT JOIN dbo.Brands AS b 
			ON b.BrandID = SA.BrandID
		LEFT JOIN dbo.CourseCategories AS cc 
			ON cc.CourseCategoryID = SA.CategoryID
		LEFT JOIN dbo.CandidateAssessments AS CA 
			ON CSA.SelfAssessmentID = CA.SelfAssessmentID 
			AND CA.DelegateUserID = @DelegateUserID
		WHERE (@CategoryId = 0 OR SA.CategoryID = @CategoryId)
			AND (SA.RetirementDate IS NULL OR SA.RetirementDate >= @Today)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.CandidateAssessments AS subCA
				INNER JOIN dbo.DelegateAccounts AS DA 
					ON subCA.DelegateUserID = DA.UserID
				WHERE subCA.SelfAssessmentID = SA.ID
					AND DA.ID = @DelegateID 
					AND subCA.RemovedDate IS NULL 
					AND subCA.CompletedDate IS NULL
			)
	)
	SELECT * 
	FROM CustomisationData
	ORDER BY CourseName
	OPTION (RECOMPILE);
END;