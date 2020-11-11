--DLSV2-95 Adds System Versioning to auditable tables (UP)

-- Remove versioning from Frameworks table
ALTER TABLE Frameworks SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE Frameworks DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE Frameworks DROP COLUMN SysStartTime;
ALTER TABLE Frameworks DROP COLUMN SysEndTime;
DROP TABLE dbo.FrameworksHistory;
GO

-- Remove versioning from FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE FrameworkCompetencyGroups DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE FrameworkCompetencyGroups DROP COLUMN SysStartTime;
ALTER TABLE FrameworkCompetencyGroups DROP COLUMN SysEndTime;
DROP TABLE dbo.FrameworkCompetencyGroupsHistory;
GO

-- Remove versioning from FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE FrameworkCompetencies DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE FrameworkCompetencies DROP COLUMN SysStartTime;
ALTER TABLE FrameworkCompetencies DROP COLUMN SysEndTime;
DROP TABLE dbo.FrameworkCompetenciesHistory;
GO

-- Remove versioning from Competencies table
ALTER TABLE Competencies SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE Competencies DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE Competencies DROP COLUMN SysStartTime;
ALTER TABLE Competencies DROP COLUMN SysEndTime;
DROP TABLE dbo.CompetenciesHistory;
GO

-- Remove versioning from CompetencyGroups table
ALTER TABLE CompetencyGroups SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE CompetencyGroups DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE CompetencyGroups DROP COLUMN SysStartTime;
ALTER TABLE CompetencyGroups DROP COLUMN SysEndTime;
DROP TABLE dbo.CompetencyGroupsHistory;
GO

-- Remove versioning from AssessmentQuestions table
ALTER TABLE AssessmentQuestions SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE AssessmentQuestions DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE AssessmentQuestions DROP COLUMN SysStartTime;
ALTER TABLE AssessmentQuestions DROP COLUMN SysEndTime;
DROP TABLE dbo.AssessmentQuestionsHistory;
GO

-- Remove versioning from AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE AssessmentQuestionLevels DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE AssessmentQuestionLevels DROP COLUMN SysStartTime;
ALTER TABLE AssessmentQuestionLevels DROP COLUMN SysEndTime;
DROP TABLE dbo.AssessmentQuestionLevelsHistory;
GO

-- Remove versioning from SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults SET (SYSTEM_VERSIONING = OFF);
ALTER TABLE SelfAssessmentResults DROP PERIOD FOR SYSTEM_TIME;
ALTER TABLE SelfAssessmentResults DROP COLUMN SysStartTime;
ALTER TABLE SelfAssessmentResults DROP COLUMN SysEndTime;
DROP TABLE dbo.SelfAssessmentResultsHistory;
GO