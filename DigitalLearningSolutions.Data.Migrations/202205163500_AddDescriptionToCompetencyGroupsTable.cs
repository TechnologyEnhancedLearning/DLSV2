namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202205163500)]
    public class AddDescriptionToCompetencyGroupsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("CompetencyGroups").AddColumn("Description").AsString(1000).Nullable();
        }
        public override void Down()
        {
            Delete.Column("Description").FromTable("CompetencyGroups");
        }
    }
}
