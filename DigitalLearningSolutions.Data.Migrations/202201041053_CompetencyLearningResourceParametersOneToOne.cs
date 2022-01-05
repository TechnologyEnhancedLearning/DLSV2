namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201041053)]
    public class CompetencyLearningResourceParametersOneToOne : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"DELETE FROM CompetencyResourceAssessmentQuestionParameters
                             WHERE ID <>
                                (SELECT MIN(ID) As ID
                                    FROM CompetencyResourceAssessmentQuestionParameters as craqp
                                    WHERE craqp.CompetencyLearningResourceID = CompetencyResourceAssessmentQuestionParameters.CompetencyLearningResourceID)");
            Create.UniqueConstraint("IX_CompetencyResourceAssessmentQuestionParameters_CompetencyLearningResourceID")
                .OnTable("CompetencyResourceAssessmentQuestionParameters").Column("CompetencyLearningResourceID");
        }
        public override void Down()
        {
            Delete.Index("IX_CompetencyResourceAssessmentQuestionParameters_CompetencyLearningResourceID")
                .OnTable("CompetencyResourceAssessmentQuestionParameters");
        }
    }
}
