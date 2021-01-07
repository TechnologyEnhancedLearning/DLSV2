INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (1,1,1,2)
	  GO
INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (2,2,1,2)
	  GO
INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (3,3,1,2)
	  GO
INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (4,4,1,2)
	  GO
INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (5,5,1,2)
	  GO
INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (6,6,1,2)
	  GO
INSERT INTO [FrameworkCompetencyGroups]
  ([CompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
	  ,[FrameworkID])
	  VALUES
	  (7,7,1,2)
	  GO
INSERT INTO [FrameworkCompetencies]
([CompetencyID]
      ,[FrameworkCompetencyGroupID]
      ,[Ordering]
      ,[UpdatedByAdminID]
      ,[FrameworkID])
(SELECT CompetencyID, CompetencyGroupID AS FrameworkCompetencyGroupID, CompetencyID AS Ordering, 1 AS UpdatedByAdminID, 2 AS FrameworkID
FROM   SelfAssessmentStructure
WHERE (SelfAssessmentID = 1))