
using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
  using FluentMigrator;

    [Migration(202607081170)]
    public class _202608051545_TD_7596_Alter_StoreDiagScoreSCO : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7596_Alter_StoreDiagScoreSCO_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7596_Alter_StoreDiagScoreSCO_Down);
        }
    }
}
