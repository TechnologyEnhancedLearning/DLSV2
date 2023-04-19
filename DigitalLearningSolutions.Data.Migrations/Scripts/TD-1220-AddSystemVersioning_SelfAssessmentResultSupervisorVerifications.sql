--TD-1220-AddSystemVersioning_SelfAssessmentResultSupervisorVerifications
--Add versioning in SelfAssessmentResultSupervisorVerifications table
ALTER TABLE SelfAssessmentResultSupervisorVerifications
    ADD
        SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN
            CONSTRAINT DF_SelfAssessmentResultSupervisorVerifications_SysStart DEFAULT SYSUTCDATETIME()
      , SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN
            CONSTRAINT DF_SelfAssessmentResultSupervisorVerifications_SysEnd DEFAULT CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'),
        PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
GO
ALTER TABLE SelfAssessmentResultSupervisorVerifications
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.SelfAssessmentResultSupervisorVerificationsHistory));
GO
