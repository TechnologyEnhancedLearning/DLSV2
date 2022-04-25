BEGIN TRANSACTION

BEGIN TRY

	SET IDENTITY_INSERT [dbo].[FrameworkCompetencyGroups] ON 

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1095, 1080, 3, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1096, 1081, 4, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1097, 1082, 5, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1098, 1083, 6, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1099, 1084, 8, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1100, 1085, 9, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1102, 1087, 11, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1103, 1088, 12, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1104, 1089, 13, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1105, 1090, 14, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1106, 1091, 15, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1107, 1092, 16, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1108, 1093, 17, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1161, 1146, 1, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1162, 1147, 2, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1163, 1148, 7, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1164, 1149, 10, 1, 2069)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1167, 1152, 1, 1, 2072)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1168, 1153, 2, 1, 2072)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1169, 1154, 3, 1, 2072)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1170, 1155, 4, 1, 2072)

		INSERT [dbo].[FrameworkCompetencyGroups] ([ID], [CompetencyGroupID], [Ordering], [UpdatedByAdminID], [FrameworkID]) VALUES (1171, 1156, 5, 1, 2072)

		SET IDENTITY_INSERT [dbo].[FrameworkCompetencyGroups] OFF
		
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