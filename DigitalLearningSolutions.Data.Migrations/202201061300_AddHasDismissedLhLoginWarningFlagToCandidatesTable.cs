namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201061300)]
    public class AddHasDismissedLhLoginWarningFlagToCandidatesTable : Migration
    {
        public override void Up()
        {
            Alter.Table("Candidates").AddColumn("HasDismissedLhLoginWarning").AsBoolean().NotNullable()
                .WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("HasDismissedLhLoginWarning").FromTable("Candidates");
        }
    }
}
