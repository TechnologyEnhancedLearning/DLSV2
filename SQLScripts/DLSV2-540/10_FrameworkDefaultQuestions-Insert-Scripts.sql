BEGIN TRANSACTION

	BEGIN TRY

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2069, 21)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2069, 144)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2069, 146)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2069, 147)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2072, 20)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2072, 21)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2072, 146)

		INSERT [dbo].[FrameworkDefaultQuestions] ([FrameworkId], [AssessmentQuestionId]) VALUES (2072, 147)

		
	COMMIT TRANSACTION

    
	END TRY
		
	BEGIN CATCH   
 
	   DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
			@Severity int = ERROR_SEVERITY(),
			@State smallint = ERROR_STATE()
	 
	   RAISERROR (@Message, @Severity, @State)
	   ROLLBACK TRANSACTION
   
  END CATCH
GO
