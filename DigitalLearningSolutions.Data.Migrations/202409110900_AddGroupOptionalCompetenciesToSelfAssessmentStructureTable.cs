
namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202409110900)]
    public  class AddGroupOptionalCompetenciesToSelfAssessmentStructureTable : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentStructure")
                .AddColumn("GroupOptionalCompetencies")
                .AsCustom("BIT")
                .NotNullable()
                .WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("GroupOptionalCompetencies").FromTable("SelfAssessmentStructure");
        }
    }
}
