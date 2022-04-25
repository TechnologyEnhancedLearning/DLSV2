BEGIN TRANSACTION

	BEGIN TRY

			SET IDENTITY_INSERT [dbo].[CandidateAssessments] ON

			INSERT [dbo].[CandidateAssessments] ([CandidateID], [SelfAssessmentID], [StartedDate], [LastAccessed], [CompleteByDate], 
			[UserBookmark], [UnprocessedUpdates], [LaunchCount], [CompletedDate], [RemovedDate], [RemovalMethodID], [EnrolmentMethodId], 
			[EnrolledByAdminId], [ID], [SubmittedDate]) 
			VALUES (254480, 4, CAST(N'2021-06-18T14:31:44.333' AS DateTime), CAST(N'2022-01-12T10:22:43.050' AS DateTime), NULL, NULL, 
			1, 370, NULL, CAST(N'2021-12-14T00:00:00.000' AS DateTime), 1, 1, NULL, 3, CAST(N'2021-11-30T09:55:47.540' AS DateTime))
			
			INSERT [dbo].[CandidateAssessments] ([CandidateID], [SelfAssessmentID], [StartedDate], [LastAccessed], [CompleteByDate], 
			[UserBookmark], [UnprocessedUpdates], [LaunchCount], [CompletedDate], [RemovedDate], [RemovalMethodID], [EnrolmentMethodId], 
			[EnrolledByAdminId], [ID], [SubmittedDate]) 
			VALUES (254480, 5, CAST(N'2020-09-09T09:28:54.557' AS DateTime), CAST(N'2021-12-15T10:38:48.017' AS DateTime), NULL, NULL, 
			0, 11, NULL, CAST(N'2021-07-22T14:00:24.507' AS DateTime), 2, 1, NULL, 4, NULL)

			INSERT [dbo].[CandidateAssessments] ([CandidateID], [SelfAssessmentID], [StartedDate], [LastAccessed], 
			[CompleteByDate], [UserBookmark], [UnprocessedUpdates], [LaunchCount], [CompletedDate], [RemovedDate], 
			[RemovalMethodID], [EnrolmentMethodId], [EnrolledByAdminId], [ID], [SubmittedDate]) 
			VALUES (11, 5, CAST(N'2020-09-09T09:28:54.557' AS DateTime), 
			CAST(N'2021-12-15T10:38:48.017' AS DateTime), 
			NULL, NULL, 0, 11, NULL, CAST(N'2021-07-22T14:00:24.507' AS DateTime), 
			2, 1, NULL, 5, NULL)
			
		
			SET IDENTITY_INSERT [dbo].[CandidateAssessments] OFF
			COMMIT TRANSACTION

	END TRY
  BEGIN CATCH   
 
	   DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
			   @Severity int = ERROR_SEVERITY(),
			   @State smallint = ERROR_STATE()
	 
	   RAISERROR(@Message, @Severity, @State);
		ROLLBACK TRANSACTION
  END CATCH
