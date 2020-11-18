--DLSV2-95 Adds System Versioning to auditable tables (UP)

--Frameworks table
ALTER TABLE Frameworks
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_Frameworks_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_Frameworks_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE Frameworks
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.FrameworksHistory));

--FrameworkCompetencyGroups table
ALTER TABLE FrameworkCompetencyGroups
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_FrameworkCompetencyGroups_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_FrameworkCompetencyGroups_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE FrameworkCompetencyGroups
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.FrameworkCompetencyGroupsHistory));

--FrameworkCompetencies table
ALTER TABLE FrameworkCompetencies
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_FrameworkCompetencies_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_FrameworkCompetencies_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE FrameworkCompetencies
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.FrameworkCompetenciesHistory));

--Competencies table
ALTER TABLE Competencies
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_Competencies_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_Competencies_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE Competencies
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.CompetenciesHistory));

--CompetencyGroups table
ALTER TABLE CompetencyGroups
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_CompetencyGroups_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_CompetencyGroups_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE CompetencyGroups
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.CompetencyGroupsHistory));

--AssessmentQuestions table
ALTER TABLE AssessmentQuestions
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_AssessmentQuestions_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_AssessmentQuestions_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE AssessmentQuestions
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.AssessmentQuestionsHistory));

--AssessmentQuestionLevels table
ALTER TABLE AssessmentQuestionLevels
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_AssessmentQuestionLevels_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_AssessmentQuestionLevels_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE AssessmentQuestionLevels
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.AssessmentQuestionLevelsHistory));

--SelfAssessmentResults table
ALTER TABLE SelfAssessmentResults
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_SelfAssessmentResults_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_SelfAssessmentResults_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO

ALTER TABLE SelfAssessmentResults
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.SelfAssessmentResultsHistory));