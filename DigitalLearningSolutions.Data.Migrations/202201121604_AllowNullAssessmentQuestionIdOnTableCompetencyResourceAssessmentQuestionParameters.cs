namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201121604)]
    public class _202201121604_AllowNullAssessmentQuestionIdOnTableCompetencyResourceAssessmentQuestionParameters : Migration
    {
        public override void Up()
        {
            Alter.Column("AssessmentQuestionID")
                .OnTable("CompetencyResourceAssessmentQuestionParameters")
                .AsInt32()
                .Nullable();
        }
        public override void Down()
        {
            Delete.FromTable("CompetencyResourceAssessmentQuestionParameters")
                .Row(new { AssessmentQuestionID = (int?)null });

            Alter.Column("AssessmentQuestionID")
                .OnTable("CompetencyResourceAssessmentQuestionParameters")
                .AsInt32()
                .NotNullable();
        }
    }
}
