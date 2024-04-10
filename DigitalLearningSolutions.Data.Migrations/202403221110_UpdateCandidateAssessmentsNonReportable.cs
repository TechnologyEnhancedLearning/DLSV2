namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202403221110)]
    public class UpdateCandidateAssessmentsNonReportable : Migration
    {
        public override void Up()
        {
            Execute.Sql(
                @$"UPDATE CandidateAssessments
                    SET NonReportable= 1
                    FROM CandidateAssessments AS CA
                    INNER JOIN CandidateAssessmentSupervisors AS CAS ON CA.ID = cas.CandidateAssessmentID AND CAS.Removed IS NULL
                    INNER JOIN SupervisorDelegates AS SD ON SD.ID = CAS.SupervisorDelegateId
                    INNER JOIN AdminAccounts AS AA ON AA.ID = SD.SupervisorAdminID AND AA.UserID = SD.DelegateUserID
                    WHERE NonReportable = 0"
                );
        }
        public override void Down()
        {
        }
    }
}
