namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202110150848)]
    public class ReorderCompetencyAssessmentQuestionsSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_379_ReorderCompetencyAssessmentQuestionsSP);
        }
        public override void Down()
        {
            Execute.Sql("DROP PROCEDURE [dbo].[ReorderCompetencyAssessmentQuestion]");
        }
    }
}
