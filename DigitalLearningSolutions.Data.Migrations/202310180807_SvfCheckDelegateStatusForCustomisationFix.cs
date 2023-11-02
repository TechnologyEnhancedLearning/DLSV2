namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202310180807)]
    internal class SvfCheckDelegateStatusForCustomisationFix : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_3000_CheckDelegateStatusForCustomisationFix_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3000_CheckDelegateStatusForCustomisationFix_down);
        }
    }
}
