namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202101180810)]
    public class AddFrameworkDefaultQuestionsTable : Migration
    {
        public override void Up()
        {
            Create.Table("FrameworkDefaultQuestions")
                .WithColumn("FrameworkId").AsInt32().NotNullable().PrimaryKey().ForeignKey("Frameworks", "ID")
                .WithColumn("AssessmentQuestionId").AsInt32().NotNullable().PrimaryKey().ForeignKey("AssessmentQuestions", "ID");
        }

        public override void Down()
        {
            Delete.Table("FrameworkDefaultQuestions");
        }
    }
}
