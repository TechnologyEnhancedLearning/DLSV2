/****** Object:  StoredProcedure [dbo].[InsertCustomisation_V3]    Script Date: 20/11/2020 14:12:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 28 February 2020
-- V2 Adds @CCCompletion field
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[InsertCustomisation_V3] 
	@Active as bit,
	@ApplicationID as int,
	@CentreID as int,
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
	@CCCompletion nvarchar(500),
	@ApplyLPDefaultsToSelfEnrol as bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @_ReturnCode Int
	if exists (SELECT CustomisationID FROM dbo.Customisations WHERE [ApplicationID] = @ApplicationID AND [CentreID] = @CentreID AND [CustomisationName] = @CustomisationName)
	BEGIN
	set @_ReturnCode = -1	
	goto OnExit
	end
	BEGIN TRY
		BEGIN TRANSACTION AddCustomisation
		--
			-- Insert a new customisation
			--
			INSERT INTO dbo.Customisations 
				(
				 [CurrentVersion], 
				 [CentreID], 
				 [ApplicationID], 
				 Active, 
CustomisationName, 
[Password], 
SelfRegister, 
TutCompletionThreshold, 
IsAssessed, 
DiagCompletionThreshold, 
DiagObjSelect, 
HideInLearnerPortal, 
CompleteWithinMonths, 
Mandatory, 
ValidityMonths, 
AutoRefresh, 
RefreshToCustomisationID, 
AutoRefreshMonths, 
Q1Options, 
Q2Options, 
Q3Options, 
CourseField1PromptID, 
CourseField2PromptID, 
CourseField3PromptID,
InviteContributors,
NotificationEmails,
ApplyLPDefaultsToSelfEnrol) 
			VALUES (1, @CentreID, @ApplicationID, @Active, 
@CustomisationName, 
@Password, 
@SelfRegister, 
@TutCompletionThreshold, 
@IsAssessed, 
@DiagCompletionThreshold, 
@DiagObjSelect, 
@HideInPortal, 
@CompleteWithinMonths, 
@Mandatory, 
@ValidityMonths, 
@AutoRefresh, 
@RefreshToCustomisationID, 
@AutoRefreshMonths, 
@Q1Options, 
@Q2Options, 
@Q3Options, 
@CourseField1PromptID, 
@CourseField2PromptID, 
@CourseField3PromptID,
@InviteContributors,
@CCCompletion,
@ApplyLPDefaultsToSelfEnrol);

		
		declare @CustomisationID integer
		Set @CustomisationID = (SELECT SCOPE_IDENTITY())
		PRINT @CustomisationID

		COMMIT TRANSACTION AddCustomisation
		--
		-- Decide what the return code should be - depends on whether they
		-- need to be approved or not
		--
		set @_ReturnCode = 	@CustomisationID				-- assume that user is registered
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION AddCustomisation
	END CATCH
	--
	-- Return code indicates errors or success
	--
	OnExit:
	SELECT @_ReturnCode
END
GO