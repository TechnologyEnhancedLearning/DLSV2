namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202011201416)]
    public class AddInsertCustomisationV3sp : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_106_CreateOrAlterInsertCustomisation_V3);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_106_DropInsertCustomisation_V3);
        }
    }
}
