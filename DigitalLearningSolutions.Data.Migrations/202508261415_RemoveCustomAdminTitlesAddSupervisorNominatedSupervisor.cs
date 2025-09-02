namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202508261415)]
    public class RemoveCustomAdminTitlesAddSupervisorNominatedSupervisor : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql($@"
                    -- 1. Drop old FK
                    ALTER TABLE [dbo].[SelfAssessmentSupervisorRoles]
                    DROP CONSTRAINT [FK_SelfAssessmentSupervisorRoles_SelfAssessmentID_SelfAssessments_ID];


                    -- 2. Make column nullable
                    ALTER TABLE [dbo].[SelfAssessmentSupervisorRoles]
                    ALTER COLUMN SelfAssessmentID INT NULL;


                    -- 3. Recreate FK (allows NULLs by default)
                    ALTER TABLE [dbo].[SelfAssessmentSupervisorRoles]
                    ADD CONSTRAINT [FK_SelfAssessmentSupervisorRoles_SelfAssessmentID_SelfAssessments_ID]
                    FOREIGN KEY (SelfAssessmentID) REFERENCES SelfAssessments(ID);


                    -- 4. Add two default roles
                    INSERT INTO [dbo].[SelfAssessmentSupervisorRoles]
                               ([SelfAssessmentID],[RoleName],[SelfAssessmentReview],[ResultsReview]
                               ,[RoleDescription]
                               ,[AllowDelegateNomination],[AllowSupervisorRoleSelection])
                         VALUES
                               (NULL,'Supervisor',1,1
		                       ,'This person may be the  line manager, practice educator or educational supervisor at University.'
		                       ,0,1),
                               (NULL,'Nominated Supervisor',0,1
		                       ,'Nominated Supervisors must be deemed competent to administer intravenous medication by their home organisation. Nominated Supervisors should be authorised to supervise and assess the practice of others by their line manager, who should consider their level of experience.'
		                       ,1,0);


                    -- 5. Update all CandidateAssessmentSupervisor records
                    UPDATE cas
                    SET cas.SelfAssessmentSupervisorRoleID =
                        CASE
                            WHEN sasr.RoleName = 'Educator/Manager' THEN sup.ID
                            WHEN sasr.RoleName = 'Assessor'         THEN nom.ID
                        END
                    FROM CandidateAssessmentSupervisors cas
                    INNER JOIN SelfAssessmentSupervisorRoles sasr 
                        ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                    LEFT JOIN SelfAssessmentSupervisorRoles sup 
                        ON sup.RoleName = 'Supervisor'
                    LEFT JOIN SelfAssessmentSupervisorRoles nom 
                        ON nom.RoleName = 'Nominated Supervisor';


                    -- 6. Remove all existing roles where SelfAssessmentID is not null
                    DELETE FROM SelfAssessmentSupervisorRoles WHERE SelfAssessmentID IS NOT NULL;
                "
            );
        }
    }
}
