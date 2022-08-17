namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202208101435)]
    public class AddMaxSignpostedResourcesConfigValue : Migration
    {
        private const string MaxSignpostedResourcesValue = "150";

        public override void Up()
        {
            Execute.Sql(
                @$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'MaxSignpostedResources')
                    BEGIN
                      INSERT INTO Config VALUES ('MaxSignpostedResources', '{MaxSignpostedResourcesValue}', 0)
                    END"
            );
        }

        public override void Down() { }
    }
}
