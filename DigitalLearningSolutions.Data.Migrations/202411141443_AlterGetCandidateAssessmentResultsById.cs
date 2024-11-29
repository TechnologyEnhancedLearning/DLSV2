namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202411141443)]
    public class AlterGetCandidateAssessmentResultsById : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4950_AlterGetCandidateAssessmentResultsById_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_4950_AlterGetCandidateAssessmentResultsById_DOWN);
        }
    }
}
