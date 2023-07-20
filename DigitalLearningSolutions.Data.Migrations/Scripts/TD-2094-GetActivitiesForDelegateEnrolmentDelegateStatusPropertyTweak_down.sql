/****** Object:  StoredProcedure [dbo].[GetActivitiesForDelegateEnrolment]    Script Date: 05/07/2023 09:33:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 24/01/2023
-- Description:	Returns active available for delegate enrolment based on original GetActiveAvailableCustomisationsForCentreFiltered_V6 sproc but adjusted for user account refactor and filters properly for category.
-- =============================================
ALTER   PROCEDURE [dbo].[GetActivitiesForDelegateEnrolment]
	-- Add the parameters for the stored procedure here
	@CentreID as Int = 0,
	@DelegateID as int,
	@CategoryId as Int = 0
AS
	BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
			SELECT * FROM
			-- Insert statements for procedure here
			(SELECT cu.CustomisationID, cu.Active, cu.CurrentVersion, cu.CentreID, cu.ApplicationID, (CASE WHEN cu.CustomisationName <> '' THEN a.ApplicationName + ' - ' + cu.CustomisationName ELSE a.ApplicationName END) AS CourseName, cu.CustomisationText, 0 AS IncludesSignposting, 0 AS IsSelfAssessment, cu.SelfRegister AS SelfRegister,
					   cu.IsAssessed, dbo.CheckCustomisationSectionHasDiagnostic(cu.CustomisationID, 0) AS HasDiagnostic, 
					   dbo.CheckCustomisationSectionHasLearning(cu.CustomisationID, 0) AS HasLearning, (SELECT BrandName FROM Brands WHERE BrandID = a.BrandID) AS Brand, (SELECT CategoryName FROM CourseCategories WHERE CourseCategoryID = a.CourseCategoryID) AS Category, (SELECT CourseTopic FROM CourseTopics WHERE CourseTopicID = a.CourseTopicID) AS Topic, dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @DelegateID) AS DelegateStatus
			FROM  Customisations AS cu INNER JOIN
					   Applications AS a 
					   ON cu.ApplicationID = a.ApplicationID 
					   WHERE ((cu.CentreID = @CentreID) OR (cu.AllCentres = 1 AND (EXISTS(SELECT CentreApplicationID FROM CentreApplications WHERE ApplicationID = a.ApplicationID AND CentreID = @CentreID AND Active = 1)))) AND 
					   (cu.Active = 1) AND 
					   (a.ASPMenu = 1) AND 
					   (a.ArchivedDate IS NULL) AND 
					   (dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @DelegateID) IN (0,1,4)) AND 
					   (cu.CustomisationName <> 'ESR') AND
					   (a.CourseCategoryID = @CategoryId OR @CategoryId =0)
					   UNION ALL
					   SELECT SA.ID AS CustomisationID, 1 AS Active, 1 AS CurrentVersion, CSA.CentreID as CentreID, 0 AS ApplicationID, SA.Name AS CourseName, SA.Description AS CustomisationText, SA.IncludesSignposting, 1 AS IsSelfAssessment, CSA.AllowEnrolment AS SelfRegister, 0 AS IsAssessed, 0 AS HasDiagnostic, 0 AS HasLearning,
										 (SELECT BrandName
										 FROM    Brands
										 WHERE (BrandID = SA.BrandID)) AS Brand,
										 (SELECT CategoryName
										 FROM    CourseCategories
										 WHERE (CourseCategoryID = SA.CategoryID)) AS Category,
										 '' AS Topic, 0 AS DelegateStatus
						FROM   SelfAssessments AS SA 
		INNER JOIN CentreSelfAssessments AS CSA ON SA.Id = CSA.SelfAssessmentID AND CSA.CentreId = @centreId AND CSA.AllowEnrolment = 1
						WHERE (SA.ID NOT IN
										 (SELECT SelfAssessmentID
										 FROM    CandidateAssessments AS CA
										 INNER JOIN Users AS U ON CA.DelegateUserID = U.ID
										 INNER JOIN DelegateAccounts AS DA ON U.ID = DA.UserID
										 WHERE (DA.ID = @DelegateID) AND (CA.RemovedDate IS NULL) AND (CA.CompletedDate IS NULL))) AND
					   (SA.CategoryID = @CategoryId OR @CategoryId =0)
										 )
										 AS Q1
						ORDER BY Q1.CourseName
		END
