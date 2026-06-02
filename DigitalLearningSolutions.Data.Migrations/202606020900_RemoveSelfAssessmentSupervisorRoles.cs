namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202606020900)]
    public class RemoveSelfAssessmentSupervisorRoles : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.key_constraints
                    WHERE name = 'IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID'
                )
                BEGIN
                    ALTER TABLE CandidateAssessmentSupervisors
                    DROP CONSTRAINT IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID
                END

                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_CandidateAssessmentSupervisors_SelfAssessmentSupervisorRoleID_SelfAssessmentSupervisorRoles_ID'
                )
                BEGIN
                    ALTER TABLE CandidateAssessmentSupervisors
                    DROP CONSTRAINT FK_CandidateAssessmentSupervisors_SelfAssessmentSupervisorRoleID_SelfAssessmentSupervisorRoles_ID
                END

                IF COL_LENGTH('CandidateAssessmentSupervisors', 'SelfAssessmentSupervisorRoleID') IS NOT NULL
                BEGIN
                    ALTER TABLE CandidateAssessmentSupervisors
                    DROP COLUMN SelfAssessmentSupervisorRoleID
                END

                IF OBJECT_ID('SelfAssessmentSupervisorRoles', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE SelfAssessmentSupervisorRoles
                END");
        }

        public override void Down()
        {
            Execute.Sql(@"
                IF OBJECT_ID('SelfAssessmentSupervisorRoles', 'U') IS NULL
                BEGIN
                    CREATE TABLE SelfAssessmentSupervisorRoles
                    (
                        ID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SelfAssessmentSupervisorRoles PRIMARY KEY,
                        SelfAssessmentID INT NOT NULL,
                        RoleName NVARCHAR(100) NOT NULL,
                        SelfAssessmentReview BIT NOT NULL CONSTRAINT DF_SelfAssessmentSupervisorRoles_SelfAssessmentReview DEFAULT 1,
                        ResultsReview BIT NOT NULL CONSTRAINT DF_SelfAssessmentSupervisorRoles_ResultsReview DEFAULT 1,
                        RoleDescription NVARCHAR(500) NULL,
                        AllowDelegateNomination BIT NOT NULL CONSTRAINT DF_SelfAssessmentSupervisorRoles_AllowDelegateNomination DEFAULT 0,
                        AllowSupervisorRoleSelection BIT NOT NULL CONSTRAINT DF_SelfAssessmentSupervisorRoles_AllowSupervisorRoleSelection DEFAULT 0,
                        CONSTRAINT FK_SelfAssessmentSupervisorRoles_SelfAssessments_SelfAssessmentID
                            FOREIGN KEY (SelfAssessmentID) REFERENCES SelfAssessments(ID)
                    )
                END

                IF COL_LENGTH('CandidateAssessmentSupervisors', 'SelfAssessmentSupervisorRoleID') IS NULL
                BEGIN
                    ALTER TABLE CandidateAssessmentSupervisors
                    ADD SelfAssessmentSupervisorRoleID INT NULL
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_CandidateAssessmentSupervisors_SelfAssessmentSupervisorRoleID_SelfAssessmentSupervisorRoles_ID'
                )
                BEGIN
                    ALTER TABLE CandidateAssessmentSupervisors
                    ADD CONSTRAINT FK_CandidateAssessmentSupervisors_SelfAssessmentSupervisorRoleID_SelfAssessmentSupervisorRoles_ID
                    FOREIGN KEY (SelfAssessmentSupervisorRoleID) REFERENCES SelfAssessmentSupervisorRoles(ID)
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.key_constraints
                    WHERE name = 'IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID'
                )
                BEGIN
                    ALTER TABLE CandidateAssessmentSupervisors
                    ADD CONSTRAINT IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID
                    UNIQUE (CandidateAssessmentID, SupervisorDelegateId, SelfAssessmentSupervisorRoleID)
                END");
        }
    }
}
