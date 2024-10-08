/****** Object:  StoredProcedure [dbo].[GroupCustomisation_Add_V2]    Script Date: 27/08/2024 14:41:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 22/10/2018
-- Description:	Adds a customisation to a group and enrols all group delegates on the customisation if applicable.
-- =============================================
ALTER PROCEDURE [dbo].[GroupCustomisation_Add_V2]
	-- Add the parameters for the stored procedure here
	@GroupID int,
	@CustomisationID int,
	@CentreID int,
	@CompleteWithinMonths int,
	@AdminUserID int,
	@CohortLearners bit,
	@SupervisorAdminID int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @_DelCount int
	Set @_DelCount = 0
	DECLARE @_EnrolCount int
	SET @_EnrolCount = 0
    -- Insert statements for procedure here
	BEGIN TRANSACTION T1
	INSERT INTO GroupCustomisations (GroupID,CustomisationID,CompleteWithinMonths,AddedByAdminUserID, CohortLearners, SupervisorAdminID)
	VALUES (@GroupID, @CustomisationID, @CompleteWithinMonths, @AdminUserID, @CohortLearners, @SupervisorAdminID)
	COMMIT TRANSACTION T1
	--check if the group contains delegates and enrol:
	select @_DelCount = Count(DelegateID) from GroupDelegates WHERE GroupID = @GroupID
	if @_DelCount > 0
	begin
	DECLARE @_DelID as Int
	SET @_DelID = 0
	declare @_ReturnCode integer
	declare @_CompleteBy datetime 
	if @CompleteWithinMonths > 0
	begin
	set @_CompleteBy = dateAdd(M,@CompleteWithinMonths,getDate())
	end
	PRINT @_CompleteBy
	WHILE @_DelID < (SELECT Max(DelegateID) FROM GroupDelegates WHERE GroupID = @GroupID)
	begin
	BEGIN TRANSACTION T2
	SELECT @_DelID = Min(DelegateID) from GroupDelegates WHERE GroupID = @GroupID AND DelegateID > @_DelID
set @_ReturnCode = 0


if (SELECT COUNT(*) FROM Customisations c WHERE (c.CustomisationID = @CustomisationID) AND ((c.CentreID = @CentreID) OR (c.AllCentres = 1 AND (EXISTS(SELECT CentreApplicationID FROM CentreApplications WHERE ApplicationID = c.ApplicationID AND CentreID = @CentreID AND Active = 1))))) = 0 
			begin
			set @_ReturnCode = 100
			
			end
			if (SELECT COUNT(*) FROM Candidates c WHERE (c.CandidateID = @_DelID) AND (c.CentreID = @CentreID)) = 0 
			begin
			set @_ReturnCode = 101
			
			end
			-- This is being changed (18/09/2018) to check if existing progress hasn't been refreshed or removed:
			if (SELECT COUNT(*) FROM Progress WHERE (CandidateID = @_DelID) AND (CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)) > 0 
			begin
			-- A record exists, should we set the Complete By Date?
			UPDATE Progress SET CompleteByDate = @_CompleteBy, EnrollmentMethodID = 3 WHERE (CandidateID = @_DelID) AND (CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL) AND (CompleteByDate IS NULL)
			set @_ReturnCode = 102
		
			end
		-- Insert record into progress
		if @_ReturnCode = 0
		begin
		EXECUTE  [dbo].[uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2] 
   @_DelID
  ,@CustomisationID
  ,@CentreID
  ,3
  ,@AdminUserID
  ,@CompleteWithinMonths
  ,@SupervisorAdminID
 SET @_EnrolCount = @_EnrolCount+1
		end
		COMMIT TRANSACTION T2
	end
	
	end
	SELECT  @_EnrolCount
	RETurn @_EnrolCount

END
GO
