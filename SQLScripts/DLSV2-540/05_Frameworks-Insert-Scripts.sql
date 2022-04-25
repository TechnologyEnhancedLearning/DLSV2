BEGIN TRANSACTION

BEGIN TRY

	SET IDENTITY_INSERT [dbo].[Frameworks] ON 

	INSERT [dbo].[Frameworks] ([ID], [BrandID], [CategoryID], [TopicID], [FrameworkName], [Description], [FrameworkConfig], [OwnerAdminID], [UpdatedByAdminID]) VALUES (2069, 6, 1, 1, N'IV Therapy Passport', NULL, N'Proficiency', 1, 1)

	INSERT [dbo].[Frameworks] ([ID], [BrandID], [CategoryID], [TopicID], [FrameworkName], [Description], [FrameworkConfig], [OwnerAdminID], [UpdatedByAdminID]) VALUES (2072, 6, 1, 1, N'Adult Critical Care IV Proficiency Passport', N'<p>This tool should be used to record practical assessments for the Adult Critical Care Intravenous Medication Passport.</p><p>Once all sections are completed, the passport forms a record that the theory and knowledge, calculation ability and practical skills of the holder have been
	demonstrated and assessed. This indicates that the holder is competent to safely administer intravenous medication in Critical Care areas.</p><p>These competencies are not exhaustive, and cover basic safety principles for some commonly used drugs. Specifically, it does not cover the use of blood
	products, total parenteral nutrition, epidurals and patient controlled analgesia or anticoagulation for extracorporeal circuits.</p>', N'Competency', 1, 1)

	SET IDENTITY_INSERT [dbo].[Frameworks] OFF

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