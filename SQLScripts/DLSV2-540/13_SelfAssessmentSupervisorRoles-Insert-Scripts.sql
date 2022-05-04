
BEGIN TRANSACTION
	BEGIN TRY	
		SET IDENTITY_INSERT [dbo].[SelfAssessmentSupervisorRoles] ON 

		INSERT [dbo].[SelfAssessmentSupervisorRoles] ([ID], [SelfAssessmentID], [RoleName], [SelfAssessmentReview], [ResultsReview], [RoleDescription], [AllowDelegateNomination]) VALUES (1, 4, N'Educator/Manager', 1, 1, N'This person may be the
		line manager, practice educator or educational supervisor at University.', 1)

		INSERT [dbo].[SelfAssessmentSupervisorRoles] ([ID], [SelfAssessmentID], [RoleName], [SelfAssessmentReview], [ResultsReview], [RoleDescription], [AllowDelegateNomination]) VALUES (2, 4, N'Assessor', 0, 1, N'Assessors must be deemed competent to administer intravenous medication by their home organisation. Assessors should be authorised to supervise and assess the practice of others by their line manager, who should consider their level of experience.', 1)

		INSERT [dbo].[SelfAssessmentSupervisorRoles] ([ID], [SelfAssessmentID], [RoleName], [SelfAssessmentReview], [ResultsReview], [RoleDescription], [AllowDelegateNomination]) VALUES (3, 5, N'Educator/Manager', 1, 1, N'This person may be the
		line manager, practice educator or educational supervisor at University.', 1)

		INSERT [dbo].[SelfAssessmentSupervisorRoles] ([ID], [SelfAssessmentID], [RoleName], [SelfAssessmentReview], [ResultsReview], [RoleDescription], [AllowDelegateNomination]) VALUES (4, 5, N'Assessor', 0, 1, N'Assessors must be deemed competent to administer intravenous medication by their home organisation. Assessors should be authorised to supervise and assess the practice of others by their line manager, who should consider their level of experience.', 1)

		SET IDENTITY_INSERT [dbo].[SelfAssessmentSupervisorRoles] OFF

		COMMIT TRANSACTION
		
	END TRY
	BEGIN CATCH   
 
		DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
           @Severity int = ERROR_SEVERITY(),
           @State smallint = ERROR_STATE()
 
		RAISERROR(@Message, @Severity, @State);
		ROLLBACK TRANSACTION
	END CATCH