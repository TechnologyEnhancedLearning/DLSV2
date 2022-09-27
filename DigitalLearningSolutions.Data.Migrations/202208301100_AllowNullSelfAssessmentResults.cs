namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202208301100)]
    public class AllowNullSelfAssessmentResults : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentResults").AlterColumn("Result").AsInt32().Nullable();
        }
        public override void Down()
        {
            Alter.Table("SelfAssessmentResults").AlterColumn("Result").AsInt32().NotNullable().SetExistingRowsTo(0);
        }
    }
}
