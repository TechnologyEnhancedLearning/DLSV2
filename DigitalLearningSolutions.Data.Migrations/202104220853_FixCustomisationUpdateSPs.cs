namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202104220853)]
    public class FixCustomisationUpdateSPs : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.dlsv2_172_fixcustomisationtutorialprogressissue_up);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.dlsv2_172_fixcustomisationtutorialprogressissue_down);
        }
    }
}
