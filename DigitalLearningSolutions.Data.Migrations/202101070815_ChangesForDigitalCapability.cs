namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202101070815)]
    public class ChangesForDigitalCapability : Migration
    {
        public override void Up()
        {
            Alter.Table("AssessmentQuestions").AddColumn("MinValue").AsInt32().NotNullable().WithDefaultValue(0);
            Alter.Table("AssessmentQuestions").AddColumn("MaxValue").AsInt32().NotNullable().WithDefaultValue(10);
            Alter.Table("AssessmentQuestions").AddColumn("ScoringInstructions").AsString(500).Nullable();
            Execute.Sql(Properties.Resources.DLSV2_133_AdjustScoresForFilteredSP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_133_UnAdjustScoresForFilteredSP);
            Delete.Column("MinValue").FromTable("AssessmentQuestions");
            Delete.Column("MaxValue").FromTable("AssessmentQuestions");
            Delete.Column("ScoringInstructions").FromTable("AssessmentQuestions");
        }
    }
}
