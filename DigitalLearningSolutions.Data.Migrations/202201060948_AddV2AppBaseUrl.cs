namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202201060948)]
    public class AddV2AppBaseUrlToConfig : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Config").Row(
                new
                {
                    ConfigName = "V2AppBaseUrl",
                    ConfigText = "https://hee-dls-test.softwire.com/", isHtml = 0
                }
            );
        }

        public override void Down()
        {
            Delete.FromTable("Config").Row(new { ConfigName = "V2AppBaseUrl" });
        }
    }
}
