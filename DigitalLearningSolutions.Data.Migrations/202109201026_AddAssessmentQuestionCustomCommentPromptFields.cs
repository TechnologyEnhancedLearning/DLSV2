namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109201026)]
    public class AddAssessmentQuestionCustomCommentPromptFields : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AssessmentQuestions").AddColumn("CommentsPrompt").AsString(50).Nullable()
                .AddColumn("CommentsHint").AsString(255).Nullable();
        }
    }
}
