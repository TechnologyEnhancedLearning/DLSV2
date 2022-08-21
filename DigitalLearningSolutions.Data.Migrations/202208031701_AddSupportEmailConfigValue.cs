namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202208031701)]
    public class AddSupportEmailConfigValue : Migration
    {
        private string supportEmailValue = "dls@hee.nhs.uk";

        public override void Up()
        {
            Execute.Sql(@$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'SupportEmail')
                        BEGIN
                          INSERT INTO Config VALUES ('SupportEmail', '{supportEmailValue}', 0)
                        END");
        }

        public override void Down()
        {

        }
    }
}
