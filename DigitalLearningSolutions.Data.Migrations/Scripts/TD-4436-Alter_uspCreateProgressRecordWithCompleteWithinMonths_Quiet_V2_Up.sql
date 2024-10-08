/****** Object:  StoredProcedure [dbo].[uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2]    Script Date: 27/08/2024 15:06:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 11/04/2019
-- Description:	Creates the Progress and aspProgress record for a new user with no return value
-- Returns:		Nothing

-- =============================================
ALTER PROCEDURE [dbo].[uspCreateProgressRecordWithCompleteWithinMonths_Quiet_V2]
	@CandidateID int,
	@CustomisationID int,
	@CentreID int,
	@EnrollmentMethodID int,
	@EnrolledByAdminID int,
	@CompleteWithinMonths int,
	@SupervisorAdminID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @EmailProfileName nvarchar(100)
	declare @TestOnly bit
	set @EmailProfileName = 'itsp-mail-profile'
	set @TestOnly = 1
	
	declare @_CompleteBy datetime 

	if @CompleteWithinMonths > 0
	begin
		set @_CompleteBy = dateAdd(M,@CompleteWithinMonths,getDate())
	end
	PRINT @_CompleteBy
	
		
		--
		-- Check if the chosen CentreID and CustomisationID match
		--
		if (SELECT COUNT(*) FROM Customisations c WHERE (c.CustomisationID = @CustomisationID) AND ((c.CentreID = @CentreID) OR (c.AllCentres = 1 AND (EXISTS(SELECT CentreApplicationID FROM CentreApplications WHERE ApplicationID = c.ApplicationID AND CentreID = @CentreID AND Active = 1))))) = 0 	
		begin
			Goto OnExit
		end
		if (SELECT COUNT(*) FROM Candidates c WHERE (c.CandidateID = @CandidateID) AND (c.CentreID = @CentreID)) = 0 
		begin
			Goto OnExit
		end
		-- This is being changed (18/09/2018) to check if existing progress hasn't been refreshed or removed:
		if (SELECT COUNT(*) FROM Progress WHERE (CandidateID = @CandidateID) AND (CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)) > 0 
		begin
			-- A record exists, should we set the Complete By Date?
			UPDATE Progress SET EnrollmentMethodID = @EnrollmentMethodID, EnrolledByAdminID = @EnrolledByAdminID WHERE (CandidateID = @CandidateID) AND (CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)

			if @CompleteWithinMonths > 0
			begin
				UPDATE Progress SET CompleteByDate = @_CompleteBy WHERE (CandidateID = @CandidateID) AND (CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)
			end

			if @SupervisorAdminID > 0
			begin
				UPDATE Progress SET SupervisorAdminID = @SupervisorAdminID WHERE (CandidateID = @CandidateID) AND (CustomisationID = @CustomisationID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)
			end
		
			Goto OnExit
		end
		
		-- Insert record into progress
		INSERT INTO Progress
					(CandidateID, CustomisationID, CustomisationVersion, SubmittedTime, EnrollmentMethodID, EnrolledByAdminID, CompleteByDate, SupervisorAdminID)
		VALUES (@CandidateID, @CustomisationID, (SELECT C.CurrentVersion FROM Customisations As C WHERE C.CustomisationID = @CustomisationID), 
					 null, @EnrollmentMethodID, @EnrolledByAdminID, @_CompleteBy, @SupervisorAdminID)
					 
		-- Get progressID
		declare @ProgressID integer
		Set @ProgressID = (SELECT SCOPE_IDENTITY())
			
		-- Insert records into aspProgress
		INSERT INTO aspProgress
			(TutorialID, ProgressID)
			(SELECT T.TutorialID, @ProgressID as ProgressID
				FROM Customisations AS C INNER JOIN
				  Applications AS A ON C.ApplicationID = A.ApplicationID INNER JOIN
				  Sections AS S ON A.ApplicationID = S.ApplicationID INNER JOIN
				  Tutorials AS T ON S.SectionID = T.SectionID
				WHERE (C.CustomisationID = @CustomisationID))

		-- Check whether a notification is needed:
		DECLARE @bodyHTML  NVARCHAR(MAX)
		DECLARE @_EmailTo nvarchar(255)
		DECLARE @_EmailFrom nvarchar(255)
		DECLARE @_Subject nvarchar(255)
		DECLARE @_BaseURL nvarchar(255)
		SELECT @_BaseURL = ConfigText FROM Config WHERE (ConfigName = 'BaseURL')
		--the email prefix and other variables will be the same for each e-mail so set them before we loop:
		SELECT @_EmailFrom = ConfigText FROM Config WHERE (ConfigName = 'MailFromAddress')
		SET @bodyHTML = ''
		SET @_Subject = N'New Learning Portal Course Enrollment'   
			
		if @EnrollmentMethodID = 2
		begin
			if exists (Select * FROM NotificationUsers WHERE NotificationID = 10 AND CandidateID = @CandidateID)
			-- we need to e-mail delegate:
			begin
				SELECT @bodyHTML = N'<p>Dear ' +  ca.FirstName + ' ' + ca.LastName  + N'</p>'+
					'<p>This is an automated message from the Digital Learning Solutions (DLS) platform to notify you that you have been enrolled on the course <b>' + a.ApplicationName + ' - ' + cu.CustomisationName + N'</b> by a system administrator (either as an individual or as a member of a group).</p>'+
					N'<p>To login to the course directly <a href="' + @_BaseURL +'tracking/learn?centreid=' + CAST(ca.CentreID AS nvarchar) +'&customisationid='+ CAST(cu.CustomisationID AS nvarchar) +'">click here</a>.</p>'+ 
					N'<p>To login to the Learning Portal to access and complete your course <a href="' + @_BaseURL +'learningportal/current?centreid=' + CAST(ca.CentreID AS nvarchar) +'">click here</a>.</p>' + CASE WHEN CompleteByDate IS NOT NULL Then '<p>The date the course should be completed by is ' + CONVERT(VARCHAR(10), CompleteByDate, 103) + N'.</p>' ELSE '' END,
					@_EmailTo = EmailAddress
					FROM Candidates AS ca INNER JOIN
								  Progress AS p ON ca.CandidateID = p.CandidateID INNER JOIN
								  Customisations AS cu ON p.CustomisationID = cu.CustomisationID INNER JOIN
								  Applications AS a ON cu.ApplicationID = a.ApplicationID
				WHERE  (p.ProgressID = @ProgressID)
			end

		end
		
		if @EnrollmentMethodID = 3
		begin
			if exists (Select * FROM NotificationUsers WHERE NotificationID = 14 AND CandidateID = @CandidateID AND NOT @_EmailTo IS NULL)
			-- we need to e-mail delegate:
			begin
				SELECT @bodyHTML = N'<p>Dear ' +  ca.FirstName + ' ' + ca.LastName  + N'</p>'+
					'<p>This is an automated message from the Digital Learning Solutions (DLS) platform to notify you that you have been enrolled on the course <b>' + a.ApplicationName + ' - ' + cu.CustomisationName + N'</b> by the system because a previous course completion has expired.</p>'+
					N'<p>To login to the course directly <a href="' + @_BaseURL +'tracking/learn?centreid=' + CAST(ca.CentreID AS nvarchar) +'&customisationid='+ CAST(cu.CustomisationID AS nvarchar) +'">click here</a>.</p>'+ 
					N'<p>To login to the Learning Portal to access and complete your course <a href="' + @_BaseURL +'learningportal/current?centreid=' + CAST(ca.CentreID AS nvarchar) +'">click here</a>.</p>' + CASE WHEN CompleteByDate IS NOT NULL Then '<p>The date the course should be completed by is ' + CONVERT(VARCHAR(10), CompleteByDate, 103) + N'.</p>' ELSE '' END,
					@_EmailTo = EmailAddress
					FROM Candidates AS ca INNER JOIN
								  Progress AS p ON ca.CandidateID = p.CandidateID INNER JOIN
								  Customisations AS cu ON p.CustomisationID = cu.CustomisationID INNER JOIN
								  Applications AS a ON cu.ApplicationID = a.ApplicationID
				WHERE  (p.ProgressID = @ProgressID)
			end
		end
		
		if LEN(@bodyHTML) > 0 AND NOT @_EmailTo IS NULL
		begin
		--Now send the e-mail:
				 --Now add the e-mail to the e-mail out table:

			INSERT INTO [dbo].[EmailOut]
					   ([EmailTo]
					   ,[EmailFrom]
					   ,[Subject]
					   ,[BodyHTML]
					   ,[AddedByProcess])
				VALUES
				   (@_EmailTo
				   ,@_EmailFrom
				   ,@_Subject
				   ,@bodyHTML
				   ,'uspCreateProgressRecordWithCompleteWithinMonths')
		end
	--
	-- Return code indicates errors or success
	--
	OnExit:

END
GO

