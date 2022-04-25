
BEGIN TRANSACTION

BEGIN TRY

		SET IDENTITY_INSERT [dbo].[CandidateAssessmentSupervisors] ON

		INSERT [dbo].[CandidateAssessmentSupervisors] ([ID], [CandidateAssessmentID], [SupervisorDelegateId], [SelfAssessmentSupervisorRoleID]) 
		VALUES (1, 4, 8, 1)

		INSERT [dbo].[CandidateAssessmentSupervisors] ([ID], [CandidateAssessmentID], [SupervisorDelegateId], [SelfAssessmentSupervisorRoleID]) 
		VALUES (2, 5, 8, 4)

		SET IDENTITY_INSERT [dbo].[CandidateAssessmentSupervisors] OFF
		
		COMMIT TRANSACTION

END TRY
  BEGIN CATCH   
 
   DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
           @Severity int = ERROR_SEVERITY(),
           @State smallint = ERROR_STATE()
 
   RAISERROR(@Message, @Severity, @State);
    ROLLBACK TRANSACTION
  END CATCH


