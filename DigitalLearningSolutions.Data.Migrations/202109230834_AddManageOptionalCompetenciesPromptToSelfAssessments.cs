namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109230834)]
    public class AddManageOptionalCompetenciesPromptToSelfAssessments : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("ManageOptionalCompetenciesPrompt").AsString(500).Nullable();
        }
    }
}
