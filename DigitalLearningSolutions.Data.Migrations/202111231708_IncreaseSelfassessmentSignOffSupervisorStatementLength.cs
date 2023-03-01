namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111231708)]
    public class IncreaseSelfassessmentSignOffSupervisorStatementLength : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AlterColumn("SignOffSupervisorStatement").AsString(2000).Nullable();
        }

        public override void Down()
        {
            Alter.Table("SelfAssessments").AlterColumn("SignOffSupervisorStatement").AsString(1000).Nullable();
        }
    }
}
