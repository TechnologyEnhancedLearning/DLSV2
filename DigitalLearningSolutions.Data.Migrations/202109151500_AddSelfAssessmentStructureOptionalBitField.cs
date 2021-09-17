namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109151500)]
    public class AddSelfAssessmentStructureOptionalBitField : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentStructure").AddColumn("Optional").AsBoolean().WithDefaultValue(false);
        }
    }
}
