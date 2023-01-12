using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202301101342)]
    public class AddSelfAssessmentResultDelegateUserID : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentResults").AddColumn("DelegateUserID").AsInt32().NotNullable().WithDefaultValue(0);

            Execute.Sql("UPDATE SAR SET SAR.DelegateUserID = DA.UserID FROM SelfAssessmentResults SAR " +
            "INNER JOIN DelegateAccounts DA ON SAR.CandidateID = DA.ID");

            Create.ForeignKey("FK_SelfAssessmentResults_DelegateUserID_Users_ID")
            .FromTable("SelfAssessmentResults").ForeignColumn("DelegateUserID").ToTable("Users").PrimaryColumn("ID");


            Delete.ForeignKey("FK_SelfAssessmentResults_CandidateID_Candidates_CandidateID").OnTable("SelfAssessmentResults");
            Rename.Column("CandidateID").OnTable("SelfAssessmentResults").To("CandidateID_deprecated");
            Alter.Table("SelfAssessmentResults").AlterColumn("CandidateID_deprecated").AsInt32().Nullable();

        }

        public override void Down()
        {
            Delete.ForeignKey("FK_SelfAssessmentResults_DelegateUserID_Users_ID").OnTable("SelfAssessmentResults");
            Delete.Column("DelegateUserID").FromTable("SelfAssessmentResults");

            Rename.Column("CandidateID_deprecated").OnTable("SelfAssessmentResults").To("CandidateID");
            Create.ForeignKey("FK_SelfAssessmentResults_CandidateID_Candidates_CandidateID")
            .FromTable("SelfAssessmentResults").ForeignColumn("CandidateID").ToTable("DelegateAccounts").PrimaryColumn("ID");
            Alter.Table("SelfAssessmentResults").AlterColumn("CandidateID").AsInt32().NotNullable();

        }
    }
}
