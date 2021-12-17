namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202112141159)]
    public class AlterTablesToUseLearningResourceReferences : Migration
    {
        public override void Up()
        {
            Execute.Sql("DELETE FROM LearningLogItems WHERE LinkedCompetencyLearningResourceID IS NOT NULL");
            Execute.Sql("DELETE FROM CompetencyLearningResources");

            Delete.Column("LHResourceReferenceID").FromTable("CompetencyLearningResources");
            Alter.Table("CompetencyLearningResources")
                .AddColumn("LearningResourceReferenceID").AsInt32().NotNullable()
                .ForeignKey("LearningResourceReferences", "ID");

            Delete.ForeignKey("FK_LearningLogItems_LinkedCompetencyLearningResourceID_CompetencyLearningResources_ID")
                .OnTable("LearningLogItems");
            Delete.Column("LinkedCompetencyLearningResourceID").FromTable("LearningLogItems");
            Alter.Table("LearningLogItems")
                .AddColumn("LearningResourceReferenceID").AsInt32().Nullable()
                .ForeignKey("LearningResourceReferences", "ID");
        }

        public override void Down()
        {
            Delete.ForeignKey(
                "FK_CompetencyLearningResources_LearningResourceReferenceID_LearningResourceReferences_ID"
            ).OnTable("CompetencyLearningResources");
            Delete.Column("LearningResourceReferenceID").FromTable("CompetencyLearningResources");
            Alter.Table("CompetencyLearningResources").AddColumn("LHResourceReferenceID").AsInt32().Nullable();

            Delete.ForeignKey("FK_LearningLogItems_LearningResourceReferenceID_LearningResourceReferences_ID")
                .OnTable("LearningLogItems");
            Delete.Column("LearningResourceReferenceID").FromTable("LearningLogItems");
            Alter.Table("LearningLogItems")
                .AddColumn("LinkedCompetencyLearningResourceID").AsInt32().Nullable()
                .ForeignKey("CompetencyLearningResources", "ID");
        }
    }
}
