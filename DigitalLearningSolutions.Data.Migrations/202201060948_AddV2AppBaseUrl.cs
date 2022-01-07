namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201060948)]
    public class AddV2AppBaseUrlToConfig : Migration
    {
        public override void Up()
        {

            Execute.Sql(@"
            BEGIN TRAN

            IF NOT EXISTS (SELECT * FROM Config WHERE ConfigName = 'V2AppBaseUrl')
            begin
                INSERT INTO Config VALUES ('V2AppBaseUrl', 'https://hee-dls-test.softwire.com/', 0)
            end

            commit
            ");
        }

        public override void Down()
        {
            Delete.FromTable("Config").Row(new { ConfigName = "V2AppBaseUrl" });
        }
    }
}
