namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202010161445)]
    public class ApplyLPDefaultSPs :Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.ApplyLPDefaultsSPChanges);
            Create.Table("FilteredAssets")
               .WithColumn("ID").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("Title").AsString(255).NotNullable()
               .WithColumn("Description").AsString(int.MaxValue).NotNullable()
               .WithColumn("DirectUrl").AsString(255).Nullable()
               .WithColumn("Type").AsString(100).Nullable()
               .WithColumn("Provider").AsString(100).Nullable()
               .WithColumn("Duration").AsInt32().Nullable();
            Create.Table("FilteredLearningActivity")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("CandidateId").AsInt32().NotNullable().ForeignKey("Candidates", "CandidateID")
                 .WithColumn("SelfAssessmentId").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                .WithColumn("FilteredAssetID").AsInt32().NotNullable().ForeignKey("FilteredAssets", "ID")
                .WithColumn("LaunchedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("LaunchCount").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("CompletedDate").AsDateTime().Nullable()
                .WithColumn("Duration").AsInt32().Nullable()
                .WithColumn("Outcome").AsInt32().Nullable();
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.DropApplyLPDefaultsSPChanges);
        }
    }
}
