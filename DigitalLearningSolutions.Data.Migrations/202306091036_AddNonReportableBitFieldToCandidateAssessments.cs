namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202306091036)]
    public class AddNonReportableBitFieldToCandidateAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessments").AddColumn("NonReportable").AsBoolean().WithDefaultValue(false);
            Execute.Sql(
                @$"UPDATE CandidateAssessments
                    SET NonReportable=1
                    FROM CandidateAssessments AS CA
                    INNER JOIN CandidateAssessmentSupervisors AS CAS ON CA.ID = cas.CandidateAssessmentID AND CAS.Removed IS NULL
                    INNER JOIN SupervisorDelegates AS SD ON SD.ID = CAS.SupervisorDelegateId
                    INNER JOIN AdminAccounts AS AA ON AA.ID = SD.SupervisorAdminID AND AA.UserID = SD.DelegateUserID"
                );
        }
        public override void Down()
        {
            Delete.Column("NonReportable").FromTable("CandidateAssessments");
        }
    }
}
