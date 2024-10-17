namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202410091524)]
    public class UpdateCandidateAssessmentSupervisorsDuplicateRows : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@$"WITH CAS_Duplicates AS (
                        SELECT ID,
                            ROW_NUMBER() OVER (PARTITION BY CandidateAssessmentID,SupervisorDelegateId ORDER BY Id DESC) AS RowNum
                        FROM CandidateAssessmentSupervisors
	                    WHERE CandidateAssessmentSupervisors.Removed is null
                    )
                    UPDATE CandidateAssessmentSupervisors
                    SET Removed = GETUTCDATE()
                    FROM CandidateAssessmentSupervisors cas INNER JOIN 
		                    CAS_Duplicates casd ON cas.ID = casd.ID
                    WHERE casd.RowNum > 1;");
        }

    }
}
