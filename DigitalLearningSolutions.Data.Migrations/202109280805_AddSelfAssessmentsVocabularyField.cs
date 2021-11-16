namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109280805)]
    public class AddSelfAssessmentsVocabularyField : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("Vocabulary").AsString(50).Nullable();
        }
    }
}
