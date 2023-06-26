namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202306260804)]
    public class AddVersionInfoPrimaryKey : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("VersionInfo").Constraint("PK_VersionInfo").Exists())
            {
                Create.PrimaryKey("PK_VersionInfo").OnTable("VersionInfo").Column("Version");
            }
        }
        public override void Down()
        {
            Delete.PrimaryKey("PK_VersionInfo").FromTable("VersionInfo");
        }
    }
}
