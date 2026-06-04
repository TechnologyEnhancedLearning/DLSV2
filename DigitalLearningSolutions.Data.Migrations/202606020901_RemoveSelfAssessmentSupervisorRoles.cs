namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    [Migration(202606020901, TransactionBehavior.None)]
    public class RemoveSelfAssessmentSupervisorRoles : ForwardOnlyMigration
    {
        public override void Up()
        {
            // take snapshot of data before making changes to the table and SPs
            Execute.Sql(Properties.Resources.TD_5638_SnapshotData_UP);

            Execute.Sql(@$"
                    BEGIN TRY
                        BEGIN TRANSACTION;

                            ALTER TABLE [CandidateAssessmentSupervisors]
                            DROP CONSTRAINT IF EXISTS [FK_CandidateAssessmentSupervisors_SelfAssessmentSupervisorRoleID_SelfAssessmentSupervisorRoles_ID];

                            ALTER TABLE CandidateAssessmentSupervisors
                            DROP CONSTRAINT IF EXISTS [IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID];

                            IF OBJECT_ID('tempdb..#DuplicateCas') IS NOT NULL
                                DROP TABLE #DuplicateCas;

                            SELECT
                                cas1.ID AS OldCasId,
                                cas2.ID AS NewCasId
                            INTO #DuplicateCas
                            FROM [CandidateAssessmentSupervisors] cas1
                            INNER JOIN [CandidateAssessmentSupervisors] cas2
                                ON cas1.CandidateAssessmentID = cas2.CandidateAssessmentID
                                AND cas1.SupervisorDelegateId = cas2.SupervisorDelegateId
                                AND cas2.Removed IS NULL
                            WHERE cas1.Removed IS NOT NULL;

                            ;WITH CasRanks AS
                            (
                                SELECT
                                    ID,
                                    CandidateAssessmentID,
                                    SupervisorDelegateId,
                                    Removed,
                                    ROW_NUMBER() OVER
                                    (
                                        PARTITION BY CandidateAssessmentID, SupervisorDelegateId
                                        ORDER BY Removed DESC
                                    ) AS RN
                                FROM CandidateAssessmentSupervisors
                                WHERE Removed IS NOT NULL
                            )
                            INSERT INTO #DuplicateCas
                            (
                                OldCasId,
                                NewCasId
                            )
                            SELECT
                                d.ID AS OldCasId,
                                k.ID AS NewCasId
                            FROM CasRanks d
                            INNER JOIN CasRanks k
                                ON d.CandidateAssessmentID = k.CandidateAssessmentID
                               AND d.SupervisorDelegateId = k.SupervisorDelegateId
                            WHERE d.RN = 2      -- older removed record
                              AND k.RN = 1;     -- most recent removed record

                            UPDATE casv
                            SET casv.CandidateAssessmentSupervisorID = d.NewCasId
                            FROM [CandidateAssessmentSupervisorVerifications] casv
                            INNER JOIN #DuplicateCas d
                                ON casv.CandidateAssessmentSupervisorID = d.OldCasId;

                            UPDATE srsv
                            SET srsv.CandidateAssessmentSupervisorID = d.NewCasId
                            FROM [SelfAssessmentResultSupervisorVerifications] srsv
                            INNER JOIN #DuplicateCas d
                                ON srsv.CandidateAssessmentSupervisorID = d.OldCasId;

                            
                            DELETE cas
                            FROM [CandidateAssessmentSupervisors] cas
                            INNER JOIN #DuplicateCas d
                                ON cas.ID = d.OldCasId;

                            IF OBJECT_ID('tempdb..#DuplicateCas') IS NOT NULL
			                    DROP TABLE #DuplicateCas;

                            ALTER TABLE [CandidateAssessmentSupervisors]
                                DROP CONSTRAINT IF EXISTS [IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId];
                            ALTER TABLE CandidateAssessmentSupervisors
                                ADD CONSTRAINT [IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId]
                                UNIQUE (CandidateAssessmentID, SupervisorDelegateId);


                            ALTER TABLE [CandidateAssessmentSupervisors]
                                DROP COLUMN [SelfAssessmentSupervisorRoleID]

		                    IF OBJECT_ID('dbo.SelfAssessmentSupervisorRoles', 'U') IS NOT NULL 
			                    DROP TABLE dbo.SelfAssessmentSupervisorRoles;

                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                            THROW;
                    END CATCH "
               );
            // Execute SPs
            Execute.Sql(Properties.Resources.TD_5638_Alter_GetAssessmentResultsByDelegate_Up);
            Execute.Sql(Properties.Resources.TD_5638_Alter_GetCandidateAssessmentResultsById_Up);
        }
    }
}
