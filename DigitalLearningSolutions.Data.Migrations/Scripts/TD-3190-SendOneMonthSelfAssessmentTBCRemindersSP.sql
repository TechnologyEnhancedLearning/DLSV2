SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 29/11/2023
-- Description:	Uses DB mail to send reminders to delegates on self assessments with a TBC date within 1 month.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SendOneMonthSelfAssessmentTBCReminders]
	-- Add the parameters for the stored procedure here
	@EmailProfileName nvarchar(100),
	@TestOnly bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-- Create the temporary table to hold the reminder records:
    DECLARE @Reminders TABLE (
		ReminderID int not null primary key identity(1,1), 
		CandidateAssessmentID int, 
		SelfAssessmentID int, 
		StartedDate datetime, 
		LastAccessed datetime, 
		CompleteByDate datetime, 
		EnrolledBy nvarchar(255),
		EnrolledByAdminID int, 
		CentreID int, 
		DelegateUserID int,
		SelfAssessment nvarchar(255), 
		LearnerName nvarchar(100), 
		LearnerEmail nvarchar(255),
		AdminEmail nvarchar(255),
		CentreName nvarchar(255)
	)
	INSERT INTO @Reminders (
		CandidateAssessmentID,
		SelfAssessmentID,
		StartedDate,
		LastAccessed,
		CompleteByDate,
		EnrolledBy,
		EnrolledByAdminID,
		CentreID,
		DelegateUserID,
		SelfAssessment,
		LearnerName,
		LearnerEmail,
		AdminEmail,
		CentreName)
	SELECT ca.ID AS CandidateAssessmentID, 
					ca.SelfAssessmentID, 
					ca.StartedDate, 
					ca.LastAccessed, 
					ca.CompleteByDate, 
					CASE WHEN EnrolmentMethodId = 2 THEN au.FirstName + ' ' + au.LastName ELSE 'yourself' END AS EnrolledBy, 
					ca.EnrolledByAdminId, 
					ca.CentreID, 
					ca.DelegateUserID, 
		            sa.Name AS SelfAssessment, 
					du.FirstName + ' ' + du.LastName AS LearnerName, 
					du.PrimaryEmail AS LearnerEmail, 
					au.PrimaryEmail AS AdminEmail, 
					ce.CentreName
		FROM NotificationUsers AS nu RIGHT OUTER JOIN
             DelegateAccounts AS da ON nu.CandidateID = da.ID RIGHT OUTER JOIN
             CandidateAssessments AS ca INNER JOIN
             Users AS du ON ca.DelegateUserID = du.ID INNER JOIN
             Centres AS ce ON ca.CentreID = ce.CentreID ON da.CentreID = ce.CentreID AND da.UserID = du.ID LEFT OUTER JOIN
             SelfAssessments AS sa ON ca.SelfAssessmentID = sa.ID LEFT OUTER JOIN
             CandidateAssessmentSupervisorVerifications AS casv ON ca.ID = casv.ID FULL OUTER JOIN
             AdminAccounts AS aa ON ca.EnrolledByAdminId = aa.ID FULL OUTER JOIN
             Users AS au ON aa.UserID = au.ID
		WHERE (NOT (ca.CompleteByDate IS NULL)) AND 
			(ca.CompleteByDate BETWEEN GETDATE() AND DATEADD(M, 1, GETDATE())) AND 
			(ca.SubmittedDate IS NULL) AND 
			(casv.Requested IS NULL) AND 
			(ca.RemovedDate IS NULL) AND 
			(ca.CompletedDate IS NULL) AND 
			(ca.NonReportable = 0) AND 
			(nu.NotificationID = 9)
	--setup variables to be used in email loop:
		DECLARE @ReminderID     int 
		DECLARE @bodyHTML  NVARCHAR(MAX)
		DECLARE @_EmailTo nvarchar(255)
		DECLARE @_EmailCC nvarchar(255)
		DECLARE @_EmailFrom nvarchar(255)
		DECLARE @_Subject nvarchar(255)
		DECLARE @_BaseUrl nvarchar(255)
	--the email prefix and other variables will be the same for each e-mail so set them before we loop:
		SET @_EmailFrom =  N'Digital Learning Solutions Reminders <noreply@dls.nhs.uk>'
		SET @_Subject = N'REMINDER: Your self assessment is due to be completed'
		SELECT @_BaseUrl = ConfigText FROM Config WHERE ConfigName = 'V2AppBaseUrl'
	--Loop through table, sending reminder emails:
	While exists (Select * From @Reminders) 
		BEGIN
			SELECT @ReminderID = Min(ReminderID) from @Reminders
		--Now setup the e-mail full body text, populating info from the reminders table and prepending prefix and appending suffix:
		SELECT @bodyHTML = N'<p>Dear ' + LearnerName + N'</p>'+
		'<p>This is an automated reminder message from the Digital Learning Solutions platform to remind you that your self assessment <b>' + SelfAssessment + N'</b> is due to be completed soon. The date the self assessment should be completed is ' + CONVERT(VARCHAR(10), CompleteByDate, 103) + N'.</p>'+
		N'<p>You started this self assessment on ' + CONVERT(VARCHAR(10), StartedDate , 103) + N' and last accessed it on ' + CONVERT(VARCHAR(10), LastAccessed , 103) + N'. You were enrolled on this course by '+ EnrolledBy +'.</p>'+
		N'<p>To log in to the self assessment directly <a href="' + @_BaseUrl + 'LearningPortal/SelfAssessment/'+ CAST(SelfAssessmentID AS nvarchar) +'">click here</a>.</p>'+ 
		N'<p>To log in to the Learning Portal to view all of your activities <a href="' + @_BaseUrl + 'LearningPortal/Current/">click here</a>.</p>',
		@_EmailTo = LearnerEmail,
		@_EmailCC = COALESCE(AdminEmail, '')
		FROM @Reminders where ReminderID = @ReminderID 
		if @_EmailCC <> ''
		begin
			SET @bodyHTML = @bodyHTML + '<p><b>Note:</b> This message has been copied to the administrator who enrolled you on this course for their information.</p>'
		end
		--Now send the e-mail:
		if @TestOnly = 0
			BEGIN
				EXEC msdb.dbo.sp_send_dbmail @profile_name=@EmailProfileName, @recipients=@_EmailTo, @copy_recipients=@_EmailCC, @from_address=@_EmailFrom, @subject=@_Subject, @body = @bodyHTML, @body_format = 'HTML' ;	
			END
		ELSE
			BEGIN
				print @_EmailTo
				print @_EmailCC
				print @_EmailFrom
				print @_Subject
				print @bodyHTML
			END
			--Now delete this record from @Reminders
				DELETE FROM @Reminders WHERE ReminderID = @ReminderID
		END
END