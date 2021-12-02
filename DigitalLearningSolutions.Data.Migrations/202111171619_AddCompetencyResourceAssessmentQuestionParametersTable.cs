namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111171619)]
    public class AddCompetencyResourceAssessmentQuestionParametersTable : Migration
    {
        public override void Up()
        {
            Create.Table("CompetencyResourceAssessmentQuestionParameters").WithColumn("ID").AsInt32().NotNullable()
                .PrimaryKey().Identity()
                .WithColumn("CompetencyLearningResourceID").AsInt32().NotNullable()
                .ForeignKey("CompetencyLearningResources", "ID")
                .WithColumn("AssessmentQuestionID").AsInt32().NotNullable().ForeignKey("AssessmentQuestions", "ID")
                .WithColumn("MinResultMatch").AsInt32().NotNullable()
                .WithColumn("MaxResultMatch").AsInt32().NotNullable()
                .WithColumn("Essential").AsBoolean().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("CompetencyResourceAssessmentQuestionParameters");
        }
    }
}
