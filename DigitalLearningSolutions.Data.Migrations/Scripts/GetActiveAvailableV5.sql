/****** Object:  StoredProcedure [dbo].[GetActiveAvailableCustomisationsForCentreFiltered_V5]    Script Date: 14/10/2020 10:02:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 05/10/2020
-- Description:	Returns active available customisations for centre v5 adds SelfAssessments.
-- =============================================
CREATE PROCEDURE [dbo].[GetActiveAvailableCustomisationsForCentreFiltered_V5]
	-- Add the parameters for the stored procedure here
	@CentreID as Int = 0,
	@CandidateID as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT * FROM
    -- Insert statements for procedure here
	(SELECT cu.CustomisationID, cu.Active, cu.CurrentVersion, cu.CentreID, cu.ApplicationID, a.ApplicationName + ' - ' + cu.CustomisationName AS CourseName, cu.CustomisationText, 0 AS UseFilteredApi, 0 AS IsSelfAssessment,
               cu.IsAssessed, dbo.CheckCustomisationSectionHasDiagnostic(cu.CustomisationID, 0) AS HasDiagnostic, 
               dbo.CheckCustomisationSectionHasLearning(cu.CustomisationID, 0) AS HasLearning, (SELECT BrandName FROM Brands WHERE BrandID = a.BrandID) AS Brand, (SELECT CategoryName FROM CourseCategories WHERE CourseCategoryID = a.CourseCategoryID) AS Category, (SELECT CourseTopic FROM CourseTopics WHERE CourseTopicID = a.CourseTopicID) AS Topic, dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @CandidateID) AS DelegateStatus
	FROM  Customisations AS cu INNER JOIN
               Applications AS a 
			   ON cu.ApplicationID = a.ApplicationID 
			   WHERE ((cu.CentreID = @CentreID) OR (cu.AllCentres = 1 AND (EXISTS(SELECT CentreApplicationID FROM CentreApplications WHERE ApplicationID = a.ApplicationID AND CentreID = @CentreID AND Active = 1)))) AND 
			   (cu.Active = 1) AND 
			   (cu.HideInLearnerPortal = 0) AND 
			   (a.ASPMenu = 1) AND 
			   (a.ArchivedDate IS NULL) AND 
			   (dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @CandidateID) IN (0,1,4)) AND 
			   (cu.CustomisationName <> 'ESR')
			   UNION ALL
			   SELECT SA.ID AS CustomisationID, 1 AS Active, 1 AS CurrentVersion, CSA.CentreID as CentreID, 0 AS ApplicationID, SA.Name AS CourseName, SA.Description AS CustomisationText, SA.UseFilteredApi, 1 AS IsSelfAssessment, 0 AS IsAssessed, 0 AS HasDiagnostic, 0 AS HasLearning,
                                 (SELECT BrandName
                                 FROM    Brands
                                 WHERE (BrandID = SA.BrandID)) AS Brand,
                                 (SELECT CategoryName
                                 FROM    CourseCategories
                                 WHERE (CourseCategoryID = SA.CategoryID)) AS Category,
                                 (SELECT CourseTopic
                 FROM    CourseTopics
                                 WHERE (CourseTopicID = SA.TopicID)) AS Topic, 0 AS DelegateStatus
                FROM   SelfAssessments AS SA 
INNER JOIN CentreSelfAssessments AS CSA ON SA.Id = CSA.SelfAssessmentID AND CSA.CentreId = @centreId AND CSA.AllowEnrolment = 1
                WHERE (SA.ID NOT IN
                                 (SELECT SelfAssessmentID
                                 FROM    CandidateAssessments AS CA
                                 WHERE (CandidateID = @candidateId) AND (RemovedDate IS NULL) AND (CompletedDate IS NULL)))) AS Q1
				ORDER BY Q1.CourseName
END
GO
