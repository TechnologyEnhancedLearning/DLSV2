BEGIN TRANSACTION

	BEGIN TRY

		SET IDENTITY_INSERT [dbo].[AssessmentQuestionLevels] ON 

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (38, 20, 1, N'Yes', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (39, 20, 2, N'Yes', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (40, 21, 1, N'Achieved', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (41, 21, 2, N'Passed', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (278, 140, 0, N'No', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (279, 140, 1, N'Yes', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (280, 141, 0, N'No', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (281, 141, 1, N'Yes', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (286, 144, 0, N'Not started', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (287, 144, 1, N'Achieved', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (290, 146, 1, N'Not applicable', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (291, 146, 2, N'Practising', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (292, 146, 3, N'Achieved', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (293, 147, 1, N'Not applicable', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (294, 147, 2, N'Practising', NULL, 1)

			INSERT [dbo].[AssessmentQuestionLevels] ([ID], [AssessmentQuestionID], [LevelValue], [LevelLabel], [LevelDescription], [UpdatedByAdminID]) VALUES (295, 147, 3, N'Achieved', NULL, 1)

		SET IDENTITY_INSERT [dbo].[AssessmentQuestionLevels] OFF

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