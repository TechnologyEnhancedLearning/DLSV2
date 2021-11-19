namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111181348)]
    public class AddLearningLogItemCompetenciesTable : Migration
    {
        public override void Up()
        {
            Create.Table("LearningLogItemCompetencies")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("LearningLogItemID").AsInt32().NotNullable().ForeignKey("LearningLogItems", "LearningLogItemID")
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("AssociatedDate").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("LearningLogItemCompetencies");
        }
    }
}
