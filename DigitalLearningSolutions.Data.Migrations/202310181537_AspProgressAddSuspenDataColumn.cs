namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202310181537)]
    public class AspProgressAddSuspenDataColumn : Migration
    {
        public override void Up()
        {
            Alter.Table("aspProgress").AddColumn("SuspendData").AsString(4096).Nullable();
        }
        public override void Down()
        {
            Delete.Column("SuspendData").FromTable("aspProgress");
        }
    }
}
