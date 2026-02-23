namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202510221132)]
    public  class AddIncludeLearnerDeclarationPromptToSelfAssessmentsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("IncludeLearnerDeclarationPrompt").AsBoolean().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("IncludeLearnerDeclarationPrompt").FromTable("SelfAssessments");
        }
    }
}
