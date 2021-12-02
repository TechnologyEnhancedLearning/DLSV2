namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111291243)]
    public class AddRequiredToCompetencyAssessmentQuestionsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("CompetencyAssessmentQuestions").AddColumn("Required").AsBoolean().NotNullable().WithDefaultValue(true);
        }

        public override void Down()
        {
            Delete.Column("Required").FromTable("CompetencyAssessmentQuestions");
        }
    }
}
