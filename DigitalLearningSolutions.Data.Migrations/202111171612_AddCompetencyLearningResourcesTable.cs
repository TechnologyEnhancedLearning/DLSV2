namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111171612)]
    public class AddCompetencyLearningResourcesTable : Migration
    {
        public override void Up()
        {
            Create.Table("CompetencyLearningResources").WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("LHResourceReferenceID").AsInt32().Nullable()
                .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
        }

        public override void Down()
        {
            Delete.Table("CompetencyLearningResources");
        }
    }
}
