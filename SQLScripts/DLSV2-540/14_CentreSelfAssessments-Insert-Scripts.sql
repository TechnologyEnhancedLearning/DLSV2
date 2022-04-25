BEGIN TRANSACTION

	BEGIN TRY

		INSERT INTO [dbo].[CentreSelfAssessments]
           ([CentreID]
           ,[SelfAssessmentID]
           ,[AllowEnrolment])
		VALUES
           (101
           ,4
           ,1)

	INSERT INTO [dbo].[CentreSelfAssessments]
           ([CentreID]
           ,[SelfAssessmentID]
           ,[AllowEnrolment])
     VALUES
           (101
           ,5
           ,1)

	COMMIT TRANSACTION

	END TRY
	BEGIN CATCH   
 
	   DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
			   @Severity int = ERROR_SEVERITY(),
			   @State smallint = ERROR_STATE()
	 
	   RAISERROR(@Message, @Severity, @State);
	   ROLLBACK TRANSACTION
   
  END CATCH