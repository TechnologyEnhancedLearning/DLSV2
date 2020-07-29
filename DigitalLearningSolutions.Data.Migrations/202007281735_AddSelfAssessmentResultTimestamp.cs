namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202007281736)]
    public class AddSelfAssessmentResultTimestamp : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentResults").AddColumn("DateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Delete.Column("DateTime").FromTable("SelfAssessmentResults");
        }
    }
}
