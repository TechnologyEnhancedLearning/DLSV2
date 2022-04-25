
BEGIN TRANSACTION

BEGIN TRY

	SET IDENTITY_INSERT [dbo].[CandidateAssessmentOptionalCompetencies] ON

	INSERT [dbo].[CandidateAssessmentOptionalCompetencies] ([ID], [CandidateAssessmentID], [CompetencyID], [IncludedInSelfAssessment], [CompetencyGroupID]) 
	VALUES (1, 4, 1407, 1, 1083)

	INSERT [dbo].[CandidateAssessmentOptionalCompetencies] ([ID], [CandidateAssessmentID], [CompetencyID], [IncludedInSelfAssessment], [CompetencyGroupID]) 
	VALUES (2, 4, 1408, 1, 1083)

	SET IDENTITY_INSERT [dbo].[CandidateAssessmentOptionalCompetencies] OFF
	
	COMMIT TRANSACTION

END TRY
  BEGIN CATCH   
 
   DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
           @Severity int = ERROR_SEVERITY(),
           @State smallint = ERROR_STATE()
 
   RAISERROR(@Message, @Severity, @State);
   
     ROLLBACK TRANSACTION
  END CATCH