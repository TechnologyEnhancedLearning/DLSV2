-- Remove versioning from FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from Frameworks table
ALTER TABLE Frameworks SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from Competencies table
ALTER TABLE Competencies SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from CompetencyGroups table
ALTER TABLE CompetencyGroups SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from AssessmentQuestions table
ALTER TABLE AssessmentQuestions SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels SET (SYSTEM_VERSIONING = OFF);
GO

IF (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AssessmentQuestionLevelsHistory' AND COLUMN_NAME = 'LevelValueID') > 0
BEGIN
--Rename the history table column to match the transaction table column name so that we can switch versioning on in Azure SQL subscriber without issue
EXEC sp_RENAME 'AssessmentQuestionLevelsHistory.LevelValueID' , 'LevelValue', 'COLUMN'
END
GO

-- Remove versioning from SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults SET (SYSTEM_VERSIONING = OFF);
GO

-- Remove versioning from SelfAssessmentResultSupervisorVerifications table
ALTER TABLE SelfAssessmentResultSupervisorVerifications SET (SYSTEM_VERSIONING = OFF);
GO
