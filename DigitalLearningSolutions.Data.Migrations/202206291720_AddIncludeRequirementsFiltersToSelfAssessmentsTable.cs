namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206291720)]
    public class AddIncludeRequirementsFiltersToSelfAssessmentsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("IncludeRequirementsFilters").AsBoolean().NotNullable().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("IncludeRequirementsFilters").FromTable("SelfAssessments");
        }
    }
}
