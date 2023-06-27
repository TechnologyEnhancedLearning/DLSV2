-- Switch on versioning from FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].FrameworkCompetenciesHistory));
GO

-- Switch on versioning from FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].FrameworkCompetencyGroupsHistory));
GO

-- Switch on versioning from Frameworks table
ALTER TABLE Frameworks SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].FrameworksHistory));
GO

-- Switch on versioning from Competencies table
ALTER TABLE Competencies SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].CompetenciesHistory));
GO

-- Switch on versioning from CompetencyGroups table
ALTER TABLE CompetencyGroups SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].CompetencyGroupsHistory));
GO

-- Switch on versioning from AssessmentQuestions table
ALTER TABLE AssessmentQuestions SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].AssessmentQuestionsHistory));
GO

-- Switch on versioning from AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].AssessmentQuestionLevelsHistory));
GO

-- Switch on versioning from SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].SelfAssessmentResultsHistory));
GO

-- Switch on versioning from SelfAssessmentResultSupervisorVerifications table
ALTER TABLE SelfAssessmentResultSupervisorVerifications SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].SelfAssessmentResultSupervisorVerificationsHistory));
GO
