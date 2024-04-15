-- Remove period field from FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from Frameworks table
ALTER TABLE Frameworks DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from Competencies table
ALTER TABLE Competencies DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from CompetencyGroups table
ALTER TABLE CompetencyGroups DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from AssessmentQuestions table
ALTER TABLE AssessmentQuestions DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults DROP PERIOD FOR SYSTEM_TIME; 
GO

-- Remove period field from SelfAssessmentResultSupervisorVerifications table
ALTER TABLE SelfAssessmentResultSupervisorVerifications DROP PERIOD FOR SYSTEM_TIME; 
GO