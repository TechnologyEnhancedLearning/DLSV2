BEGIN TRANSACTION

	BEGIN TRY

		SET IDENTITY_INSERT [dbo].[AssessmentQuestionInputTypes] ON 

		INSERT [dbo].[AssessmentQuestionInputTypes] ([ID], [InputTypeName]) VALUES (3, N'Yes/No or True/False radio buttons')

		SET IDENTITY_INSERT [dbo].[AssessmentQuestionInputTypes] OFF
		COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
   
 
		DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
        @Severity int = ERROR_SEVERITY(),
        @State smallint = ERROR_STATE()
 
		RAISERROR (@Message, @Severity, @State)
		ROLLBACK TRANSACTION
   
	END CATCH
