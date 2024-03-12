-- Remove period field from FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from Frameworks table
ALTER TABLE Frameworks ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from Competencies table
ALTER TABLE Competencies ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from CompetencyGroups table
ALTER TABLE CompetencyGroups ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from AssessmentQuestions table
ALTER TABLE AssessmentQuestions ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO

-- Remove period field from SelfAssessmentResultSupervisorVerifications table
ALTER TABLE SelfAssessmentResultSupervisorVerifications ADD PERIOD FOR SYSTEM_TIME ( SysStartTime, SysEndTime ); 
GO