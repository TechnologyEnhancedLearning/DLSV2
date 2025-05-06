namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202504241517)]
    public class AddFieldIsPrimaryToSelfAssessmentFrameworksTable : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentFrameworks").AddColumn("IsPrimary").AsBoolean().NotNullable().WithDefaultValue(1);
        }
    }
}
