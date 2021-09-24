namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109221053)]
    public class SelfAssessmentsAddUseDescriptionExpanders : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("UseDescriptionExpanders").AsBoolean().NotNullable().WithDefaultValue(true);
        }
    }
}
