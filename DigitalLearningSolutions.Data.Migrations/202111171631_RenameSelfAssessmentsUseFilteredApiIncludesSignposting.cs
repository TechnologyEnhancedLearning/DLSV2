namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111171631)]
    public class RenameSelfAssessmentsUseFilteredApiIncludesSignposting : Migration
    {
        public override void Up()
        {
            Rename.Column("UseFilteredApi").OnTable("SelfAssessments").To("IncludesSignposting");
            Execute.Sql(Properties.Resources.HEEDLS_667_GetActiveAvailableCustomisationsForCentreFiltered_V5_Signposting_UP);
        }

        public override void Down()
        {
            Rename.Column("IncludesSignposting").OnTable("SelfAssessments").To("UseFilteredApi");
            Execute.Sql(Properties.Resources.HEEDLS_667_GetActiveAvailableCustomisationsForCentreFiltered_V5_Signposting_DOWN);
        }
    }
}
