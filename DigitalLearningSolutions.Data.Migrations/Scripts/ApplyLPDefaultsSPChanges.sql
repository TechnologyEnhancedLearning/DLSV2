
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 16/10/2020
-- Description:	Updates a customisation based on form values
-- V2 Adds @CCEmail
-- V3 Adds ApplyLPDefaultsToSelfEnrol
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCustomisation_V3]
	-- Add the parameters for the stored procedure here
	@CustomisationID As Int,
	@Active as bit,
	@CustomisationName as nvarchar(250),
	@Password as nvarchar(50),
	@SelfRegister as bit,
	@TutCompletionThreshold as int,
	@IsAssessed as bit,
	@DiagCompletionThreshold as int,
	@DiagObjSelect as bit,
	@HideInPortal as bit,
	@CompleteWithinMonths as int,
	@Mandatory as bit,
	@ValidityMonths as int,
	@AutoRefresh as bit,
	@RefreshToCustomisationID as int,
	@AutoRefreshMonths as int,
	@Q1Options as nvarchar(1000),
	@Q2Options as nvarchar(1000),
	@Q3Options as nvarchar(1000),
	@CourseField1PromptID as int,
	@CourseField2PromptID as int,
	@CourseField3PromptID as int,
	@InviteContributors as bit,
	@CCEmail as nvarchar(500),
	@ApplyLPDefaultsToSelfEnrol as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @_ReturnCode Int
	if exists (SELECT CustomisationID FROM dbo.Customisations WHERE [ApplicationID] = (SELECT ApplicationID FROM Customisations AS C2 WHERE C2.CustomisationID = @CustomisationID) AND [CentreID] = (SELECT CentreID FROM Customisations AS C3 WHERE C3.CustomisationID = @CustomisationID) AND [CustomisationName] = @CustomisationName and [CustomisationID] <> @CustomisationID)
	BEGIN
	--another customisation exists with the same name
	set @_ReturnCode = -1	
	goto OnExit
	end
    -- Insert statements for procedure here
	UPDATE       Customisations
SET                
Active = @Active, 
CustomisationName = @CustomisationName, 
Password = @Password, 
SelfRegister = @SelfRegister, 
CurrentVersion = CurrentVersion + 1, 
TutCompletionThreshold = @TutCompletionThreshold, 
IsAssessed = @IsAssessed, 
DiagCompletionThreshold = @DiagCompletionThreshold, 
DiagObjSelect = @DiagObjSelect, 
HideInLearnerPortal = @HideInPortal, 
CompleteWithinMonths = @CompleteWithinMonths, 
Mandatory = @Mandatory, 
ValidityMonths = @ValidityMonths, 
AutoRefresh = @AutoRefresh, 
RefreshToCustomisationID = @RefreshToCustomisationID, 
AutoRefreshMonths = @AutoRefreshMonths, 
Q1Options = @Q1Options, 
Q2Options = @Q2Options, 
Q3Options = @Q3Options, 
CourseField1PromptID = @CourseField1PromptID, 
CourseField2PromptID = @CourseField2PromptID, 
CourseField3PromptID = @CourseField3PromptID,
InviteContributors = @InviteContributors,
NotificationEmails = @CCEmail,
ApplyLPDefaultsToSelfEnrol = @ApplyLPDefaultsToSelfEnrol
WHERE        (CustomisationID = @CustomisationID)
SET @_ReturnCode = @CustomisationID
OnExit:
SELECT @_ReturnCode
Return @_ReturnCode
END

/****** Object:  StoredProcedure [dbo].[uspCreateProgressRecord_V3]    Script Date: 16/10/2020 13:51:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 15 February 2012
-- Description:	Creates the Progress and aspProgress record for a new user
-- Returns:		0 : success, progress created
--       		1 : Failed - progress already exists
--       		100 : Failed - CentreID and CustomisationID don't match
--       		101 : Failed - CentreID and CandidateID don't match

-- V3 changes include:

-- Checks that existing progress hasn't been Removed or Refreshed before returining error.
-- Adds parameters for Enrollment method and admin ID
-- =============================================
ALTER PROCEDURE [dbo].[uspCreateProgressRecord_V3]
	@CandidateID int,
	@CustomisationID int,
	@CentreID int,
	@EnrollmentMethodID int,
	@EnrolledByAdminID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @_ReturnCode integer
	set @_ReturnCode = 0
	BEGIN TRY
		BEGIN TRANSACTION CreateProgress
		--
		-- Check if the chosen CentreID and CustomisationID match
		--
		if (SELECT COUNT(*) FROM Customisations c WHERE (c.CustomisationID = @CustomisationID) AND (c.Active = 1) AND ((c.CentreID = @CentreID) OR (c.AllCentres = 1 AND (EXISTS(SELECT CentreApplicationID FROM CentreApplications WHERE ApplicationID = c.ApplicationID AND CentreID = @CentreID AND Active = 1))))) = 0 
			begin
			set @_ReturnCode = 100
			raiserror('Error', 18, 1)
			end
			if (SELECT COUNT(*) FROM Candidates c WHERE (c.CandidateID = @CandidateID) AND (c.CentreID = @CentreID)) = 0 
			begin
			set @_ReturnCode = 101
			raiserror('Error', 18, 1)
			end
			-- This is being changed (18/09/2018) to check if existing progress hasn't been refreshed or removed:
			if (SELECT COUNT(*) FROM Progress p WHERE (p.CandidateID = @CandidateID) AND (p.CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)) > 0 
			begin
			set @_ReturnCode = 1
			raiserror('Error', 18, 1)
			end
		-- Insert record into progress
		
		declare @_CompleteBy datetime 
		if (SELECT ApplyLPDefaultsToSelfEnrol FROM Customisations c WHERE (c.CustomisationID = @CustomisationID)) = 1
		begin
		declare @CompleteWithinMonths As int
		SELECT @CompleteWithinMonths = CompleteWithinMonths FROM Customisations c WHERE (c.CustomisationID = @CustomisationID)
		if @CompleteWithinMonths > 0
	begin
	set @_CompleteBy = dateAdd(M,@CompleteWithinMonths,getDate())
	end
		end
		INSERT INTO Progress
						(CandidateID, CustomisationID, CustomisationVersion, SubmittedTime, EnrollmentMethodID, EnrolledByAdminID, CompleteByDate)
			VALUES		(@CandidateID, @CustomisationID, (SELECT C.CurrentVersion FROM Customisations As C WHERE C.CustomisationID = @CustomisationID), 
						 GETUTCDATE(), @EnrollmentMethodID, @EnrolledByAdminID, @_CompleteBy)
		-- Get progressID
		declare @ProgressID integer
		Set @ProgressID = (SELECT SCOPE_IDENTITY())
		-- Insert records into aspProgress
		INSERT INTO aspProgress
		(TutorialID, ProgressID)
		(SELECT     T.TutorialID, @ProgressID as ProgressID
FROM         Customisations AS C INNER JOIN
                      Applications AS A ON C.ApplicationID = A.ApplicationID INNER JOIN
                      Sections AS S ON A.ApplicationID = S.ApplicationID INNER JOIN
                      Tutorials AS T ON S.SectionID = T.SectionID
WHERE     (C.CustomisationID = @CustomisationID) )
		
		--
		-- All finished
		--
		COMMIT TRANSACTION CreateProgress
		--
		-- Decide what the return code should be - depends on whether they
		-- need to be approved or not
		--
		set @_ReturnCode = 0					-- assume that user is registered
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION CreateProgress
	END CATCH
	--
	-- Return code indicates errors or success
	--
	SELECT @_ReturnCode
END

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 19/10/2020
-- Description:	Logs learner activity against Filtered Learning Assets (adding where necessary)
-- =============================================
CREATE PROCEDURE UpdateFilteredLearningActivity
	-- Add the parameters for the stored procedure here
	@FilteredAssetID int, 
	@Title nvarchar(255),
	@Description nvarchar(max),
	@DirectUrl nvarchar(100),
	@Type nvarchar(100),
	@Provider nvarchar(100),
	@Duration int,
	@ActualDuration int,
	@CandidateId int,
	@SelfAssessmentId int,
	@Completed bit,
	@Outcome nvarchar(50),
	@Bookmark bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @nOutcome int
	SET @nOutcome = CASE WHEN @Outcome = 'useful' then 1 WHEN @Outcome = 'already_know' then 2 WHEN @Outcome = 'not_relevant' then 3 ELSE 0 END

    -- Check if the asset exists in FilteredAssets table and add it if not
	If not exists (SELECT Id
                    FROM FilteredAssets
                    WHERE Id = @FilteredAssetID)
					begin
					INSERT INTO FilteredAssets
(ID
, [Title]
, [Description]
, [DirectUrl]
, [Type]
, [Provider]
, [Duration])
VALUES
(@FilteredAssetID
, @Title
, @Description
, @DirectUrl
, @Type
, @Provider
, @Duration)
					end
					-- check if matching FilteredLearningActivity already exists
					if not exists (Select * FROM FilteredLearningActivity WHERE FilteredAssetID = @FilteredAssetID AND CandidateId = @CandidateId AND SelfAssessmentId = @SelfAssessmentId)
					begin
					-- it doesn't, so insert it:

INSERT INTO [dbo].[FilteredLearningActivity]
           ([CandidateId]
           ,[SelfAssessmentId]
           ,[FilteredAssetID]
           ,[LaunchCount]
           ,[Duration]
           ,[Bookmarked])
     VALUES
           (@CandidateId
           ,@SelfAssessmentId
           ,@FilteredAssetID
           ,1
           ,@ActualDuration
           ,@Bookmark)
					end
					else
					begin
					-- it does, so update it:

UPDATE [dbo].[FilteredLearningActivity]
   SET 
      [LaunchedDate] = GETDATE()
      ,[LaunchCount] = CASE WHEN @Completed = 1 THEN [LaunchCount] ELSE [LaunchCount]+1 END
      ,[CompletedDate] = CASE WHEN @Completed = 1 THEN COALESCE([CompletedDate], GETDATE()) ELSE NULL END
      ,[Duration] = @ActualDuration
      ,[Outcome] = @nOutcome
      ,[Bookmarked] = @Bookmark
 WHERE FilteredAssetID = @FilteredAssetID AND CandidateId = @CandidateId AND SelfAssessmentId = @SelfAssessmentId

					end
END
GO
