namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202110131529)]
    public class AddSignOffStatementFieldsToSelfAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("SignOffRequestorStatement").AsString(1000).Nullable()
                .AddColumn("SignOffSupervisorStatement").AsString(1000).Nullable()
                .AlterColumn("ManageOptionalCompetenciesPrompt").AsString(1000).Nullable();
        }
        public override void Down()
        {
            Alter.Table("SelfAssessments").AlterColumn("ManageOptionalCompetenciesPrompt").AsString(500).Nullable();
            Delete.Column("SignOffRequestorStatement").FromTable("SelfAssessments");
            Delete.Column("SignOffSupervisorStatement").FromTable("SelfAssessments");
        }
    }
}
