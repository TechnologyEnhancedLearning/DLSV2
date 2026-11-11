using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202611111020)]
    public class Alter_GetCandidateAssessmentResultsById_FilterIssue : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7995_Alter_GetCandidateAssessmentResultsById_FilterIssue_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7995_Alter_GetCandidateAssessmentResultsById_FilterIssue_Down);
        }
    }
}
