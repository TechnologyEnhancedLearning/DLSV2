
BEGIN TRANSACTION
	
	BEGIN TRY	  
		  
		SET IDENTITY_INSERT [dbo].[CompetencyGroups] ON 
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1080, N'Patient', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1081, N'Planning', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1082, N'Correctly prepare medicine for the patient', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1083, N'Methods of preparation', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1084, N'Administration', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1085, N'Methods of administration', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1086, N'Respiratory', 466)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1087, N'Peripheral venous access devices (PVAD): peripheral cannula', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1088, N'Peripheral venous access devices (PVAD): midline catheter', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1089, N'Central Venous Access Devices (CVAD): central venous catheter (CVC) short term non skin tunnelled', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1090, N'Central Venous Access Devices (CVAD): central venous catheter (CVC) long term skin tunnelled', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1091, N'Central Venous Access Devices (CVAD): central venous catheter (CVC) percutaneously inserted central catheter (PICC)', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1092, N'Central venous access device (CVAD): implanted central venous catheter (port)', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1093, N'Central venous access device (CVAD): umbilical venous catheter (UVC)', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1146, N'Pre-requisite learning', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1147, N'Record of learning and knowledge assessment', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1148, N'Correctly prepare medicine for the patient continued', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1149, N'Administration continued', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1152, N'Prerequisites', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1153, N'Requirements to be completed prior to or alongside this assessment', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1154, N'Mandatory proficiencies', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1155, N'Optional proficiencies (choose one from this group)', 1)
		
		INSERT [dbo].[CompetencyGroups] ([ID], [Name], [UpdatedByAdminID]) VALUES (1156, N'Optional proficiencies (choose any, all or none from this group)', 1)
		
		SET IDENTITY_INSERT [dbo].[CompetencyGroups] OFF
		
		COMMIT TRANSACTION
		
		 END TRY
  BEGIN CATCH   
 
	   DECLARE @Message varchar(MAX) = ERROR_MESSAGE(),
			   @Severity int = ERROR_SEVERITY(),
			   @State smallint = ERROR_STATE()
	 
		RAISERROR(@Message, @Severity, @State);
		ROLLBACK TRANSACTION
  END CATCH
