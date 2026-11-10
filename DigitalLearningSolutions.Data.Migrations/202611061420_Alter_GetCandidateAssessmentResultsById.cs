using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202611061420)]
    public class Alter_GetCandidateAssessmentResultsById : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7995_Alter_GetCandidateAssessmentResultsById_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7995_Alter_GetCandidateAssessmentResultsById_Down);
        }
    }
}
