namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202110131529)]
    public class AddSignOffStatementFieldsToSelfAssessments : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("SignOffRequestorStatement").AsString(1000).Nullable()
                .AddColumn("SignOffSupervisorStatement").AsString(1000).Nullable();
        }
    }
}
