namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111081225)]
    public class ReduceGroupsGroupDescriptionLength : Migration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE Groups SET GroupDescription = LEFT(GroupDescription, 1000)");
            Alter.Table("Groups").AlterColumn("GroupDescription").AsString(1000).Nullable();
        }

        public override void Down()
        {
            Alter.Table("Groups").AlterColumn("GroupDescription").AsString(int.MaxValue).Nullable();
        }
    }
}
