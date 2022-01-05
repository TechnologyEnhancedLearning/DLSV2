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
            Delete.PrimaryKey("PK_CompetencyResourceAssessmentQuestionParameters")
                .FromTable("CompetencyResourceAssessmentQuestionParameters");
            Delete.Column("ID").FromTable("CompetencyResourceAssessmentQuestionParameters");
            Create.PrimaryKey("PK_CompetencyResourceAssessmentQuestionParameters")
                .OnTable("CompetencyResourceAssessmentQuestionParameters").Column("CompetencyLearningResourceID");
            Create.UniqueConstraint("IX_CompetencyResourceAssessmentQuestionParameters_CompetencyLearningResourceID")
                .OnTable("CompetencyResourceAssessmentQuestionParameters").Column("CompetencyLearningResourceID");
        }
        public override void Down()
        {
            Delete.PrimaryKey("PK_CompetencyResourceAssessmentQuestionParameters")
                .FromTable("CompetencyResourceAssessmentQuestionParameters");
            Delete.Index("IX_CompetencyResourceAssessmentQuestionParameters_CompetencyLearningResourceID")
                .OnTable("CompetencyResourceAssessmentQuestionParameters");
            Alter.Table("CompetencyResourceAssessmentQuestionParameters").AddColumn("ID").AsInt32().NotNullable()
                .PrimaryKey().Identity();
        }
    }
}
