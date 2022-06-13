namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202205261100)]
    public class IncreaseCompetencyGroupsDescriptionLength : Migration
    {
        public override void Up()
        {
            Alter.Table("CompetencyGroups").AlterColumn("Description").AsString(int.MaxValue).Nullable();
        }
        public override void Down()
        {
            const int OriginalDescriptionLength = 1000;

            // Truncate any description longer than 1000 characters.
            Execute.Sql($"UPDATE CompetencyGroups SET Description = LEFT(Description, {OriginalDescriptionLength})");

            Alter.Table("CompetencyGroups").AlterColumn("Description").AsString(OriginalDescriptionLength).Nullable();
        }
    }
}
