namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111231427)]
    public class IncreaseSelfassessmentSignOffRequestorStatementLength : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AlterColumn("SignOffRequestorStatement").AsString(2000).Nullable();
        }

        public override void Down()
        {
            Alter.Table("SelfAssessments").AlterColumn("SignOffRequestorStatement").AsString(1000).Nullable();
        }
    }
}
