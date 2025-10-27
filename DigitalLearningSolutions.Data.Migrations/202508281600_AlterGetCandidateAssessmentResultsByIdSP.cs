namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202508281600)]
    public class AlterGetCandidateAssessmentResultsByIdSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5638_Alter_GetCandidateAssessmentResultsById_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5638_Alter_GetCandidateAssessmentResultsById_DOWN);
        }
    }
}
