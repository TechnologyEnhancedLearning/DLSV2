using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202608171522)]
    public class Alter_uspReturnSectionsForCandCust_V2 : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7599_Alter_uspReturnSectionsForCandCust_V2_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7599_Alter_uspReturnSectionsForCandCust_V2_Down);
        }
    }
}
