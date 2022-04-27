
BEGIN TRANSACTION
	BEGIN TRY

		SET IDENTITY_INSERT [dbo].[AssessmentQuestions] ON 

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (20, N'First supervised practice completed?', NULL, NULL, 3, 1, 0, 1, NULL, 1, NULL, NULL)

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (21, N'Final assessment outcome', NULL, NULL, 3, 0, 0, 1, N'<p>First assessment must be achieved and verified before completing final assessment.</p>', 1, N'Action plan', NULL)

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (140, N'Completed?', NULL, NULL, 3, 1, 0, 1, NULL, 1, N'Date completed', NULL)

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (141, N'Passed?', NULL, NULL, 3, 1, 0, 1, NULL, 1, N'Date passed', NULL)

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (144, N'First assessment outcome', NULL, NULL, 3, 0, 0, 1, NULL, 1, NULL, NULL)

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (146, N'Optional practise assessment 1', NULL, NULL, 1, 0, 1, 3, NULL, 1, NULL, NULL)

		INSERT [dbo].[AssessmentQuestions] ([ID], [Question], [MaxValueDescription], [MinValueDescription], [AssessmentQuestionInputTypeID], [IncludeComments], [MinValue], [MaxValue], [ScoringInstructions], [AddedByAdminId], [CommentsPrompt], [CommentsHint]) VALUES (147, N'Optional practise assessment 2', NULL, NULL, 1, 0, 1, 3, NULL, 1, NULL, NULL)

		SET IDENTITY_INSERT [dbo].[AssessmentQuestions] OFF	
		
		COMMIT TRANSACTION		

	END TRY

	BEGIN CATCH   
 
		DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
		   @Severity int = ERROR_SEVERITY(),
           @State smallint = ERROR_STATE()
 
		RAISERROR (@Message, @Severity, @State)
		ROLLBACK TRANSACTION
	
  END CATCH
