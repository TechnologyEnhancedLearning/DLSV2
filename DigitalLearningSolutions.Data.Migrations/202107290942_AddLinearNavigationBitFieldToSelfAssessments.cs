namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107290942)]
    public class AddLinearNavigationBitFieldToSelfAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("LinearNavigation").AsBoolean().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("LinearNavigation").FromTable("SelfAssessments");
        }
    }
}
