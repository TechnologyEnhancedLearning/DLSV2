namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107160937)]
    public class FixGetLinkedFieldNameFunction : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_272_AlterGetLinkedFieldNameFunction_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_272_AlterGetLinkedFieldNameFunction_DOWN);
        }
    }
}
