--DLSV2-95 Removes System Versioning to auditable tables (DOWN)


-- Remove versioning from FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies SET (SYSTEM_VERSIONING = OFF);
DROP TABLE dbo.FrameworkCompetencies;
DROP TABLE dbo.FrameworkCompetenciesHistory;
GO

-- Remove versioning from FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups SET (SYSTEM_VERSIONING = OFF);
DROP TABLE dbo.FrameworkCompetencyGroups;
DROP TABLE dbo.FrameworkCompetencyGroupsHistory;
GO

-- Remove versioning from Frameworks table
ALTER TABLE Frameworks SET (SYSTEM_VERSIONING = OFF);
DROP TABLE dbo.Frameworks;
DROP TABLE dbo.FrameworksHistory;
GO

-- Remove versioning from Competencies table
ALTER TABLE Competencies SET (SYSTEM_VERSIONING = OFF);
GO
ALTER TABLE Competencies DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE [dbo].[Competencies] DROP CONSTRAINT [DF_Competencies_SysEnd];
ALTER TABLE [dbo].[Competencies] DROP CONSTRAINT [DF_Competencies_SysStart];
ALTER TABLE Competencies DROP COLUMN SysStartTime;
ALTER TABLE Competencies DROP COLUMN SysEndTime;
DROP TABLE dbo.CompetenciesHistory;
GO

-- Remove versioning from CompetencyGroups table
ALTER TABLE CompetencyGroups SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE CompetencyGroups DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE [dbo].CompetencyGroups DROP CONSTRAINT [DF_CompetencyGroups_SysEnd];
ALTER TABLE [dbo].CompetencyGroups DROP CONSTRAINT [DF_CompetencyGroups_SysStart];
ALTER TABLE CompetencyGroups DROP COLUMN SysStartTime;
ALTER TABLE CompetencyGroups DROP COLUMN SysEndTime;
DROP TABLE dbo.CompetencyGroupsHistory;
GO

-- Remove versioning from AssessmentQuestions table
ALTER TABLE AssessmentQuestions SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE AssessmentQuestions DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE [dbo].AssessmentQuestions DROP CONSTRAINT [DF_AssessmentQuestions_SysEnd];
ALTER TABLE [dbo].AssessmentQuestions DROP CONSTRAINT [DF_AssessmentQuestions_SysStart];
ALTER TABLE AssessmentQuestions DROP COLUMN SysStartTime;
ALTER TABLE AssessmentQuestions DROP COLUMN SysEndTime;
DROP TABLE dbo.AssessmentQuestionsHistory;
GO

-- Remove versioning from AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE AssessmentQuestionLevels DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE [dbo].AssessmentQuestionLevels DROP CONSTRAINT [DF_AssessmentQuestionLevels_SysEnd];
ALTER TABLE [dbo].AssessmentQuestionLevels DROP CONSTRAINT [DF_AssessmentQuestionLevels_SysStart];
ALTER TABLE AssessmentQuestionLevels DROP COLUMN SysStartTime;
ALTER TABLE AssessmentQuestionLevels DROP COLUMN SysEndTime;
DROP TABLE dbo.AssessmentQuestionLevelsHistory;
GO

-- Remove versioning from SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE SelfAssessmentResults DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE [dbo].SelfAssessmentResults DROP CONSTRAINT [DF_SelfAssessmentResults_SysEnd];
ALTER TABLE [dbo].SelfAssessmentResults DROP CONSTRAINT [DF_SelfAssessmentResults_SysStart];
ALTER TABLE SelfAssessmentResults DROP COLUMN SysStartTime;
ALTER TABLE SelfAssessmentResults DROP COLUMN SysEndTime;
DROP TABLE dbo.SelfAssessmentResultsHistory;
GO
