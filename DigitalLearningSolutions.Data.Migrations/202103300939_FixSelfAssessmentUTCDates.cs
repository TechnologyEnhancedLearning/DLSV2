namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202103300939)]
    public class FixSelfAssessmentUTCDates : Migration
    {
        public override void Up()
        {
            Alter.Column("CreatedDate").OnTable("SelfAssessments").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }
        public override void Down()
        {
            Alter.Column("CreatedDate").OnTable("SelfAssessments").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}
