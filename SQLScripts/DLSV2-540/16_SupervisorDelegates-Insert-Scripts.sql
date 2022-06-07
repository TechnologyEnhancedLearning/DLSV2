BEGIN TRANSACTION

	BEGIN TRY

			SET IDENTITY_INSERT [dbo].[SupervisorDelegates] ON

			INSERT [dbo].[SupervisorDelegates] ([ID], [SupervisorAdminID], [DelegateEmail], [CandidateID], [Added], [NotificationSent], [Removed], [SupervisorEmail], [AddedByDelegate], [InviteHash]) 
			VALUES (8, 1, N'kevin.whittaker@hee.nhs.uk', 254480, CAST(N'2021-06-28T16:40:35.507' AS DateTime), CAST(N'2021-06-28T16:40:35.507' AS DateTime), NULL, N'kevin.whittaker@hee.nhs.uk', 0, N'72e44c4d-77bd-4bed-a254-7cc27ab32927')

			INSERT [dbo].[SupervisorDelegates] ([ID], [SupervisorAdminID], [DelegateEmail], [CandidateID], [Added], [NotificationSent], [Removed], [SupervisorEmail], [AddedByDelegate], [InviteHash]) 
			VALUES (9, 1, N'louis.theroux@gmail.com', NULL, CAST(N'2021-06-29T14:48:10.397' AS DateTime), CAST(N'2021-06-29T14:48:10.397' AS DateTime), CAST(N'2021-07-02T11:42:20.563' AS DateTime), N'kevin.whittaker@hee.nhs.uk', 0, N'c4e6774a-3dd5-4641-bd5e-36cc6d25e1c8')

			SET IDENTITY_INSERT [dbo].[SupervisorDelegates] OFF
			COMMIT TRANSACTION

	END TRY
	BEGIN CATCH   
 
		DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
           @Severity int = ERROR_SEVERITY(),
           @State smallint = ERROR_STATE()
 
		RAISERROR(@Message, @Severity, @State);
		ROLLBACK TRANSACTION
	END CATCH
