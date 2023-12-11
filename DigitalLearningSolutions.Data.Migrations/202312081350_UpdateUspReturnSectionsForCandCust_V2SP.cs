namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202312081350)]
    public class UpdateUspReturnSectionsForCandCust_V2SP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_2481_Update_uspReturnSectionsForCandCust_V2_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_2481_Update_uspReturnSectionsForCandCust_V2_down);
        }
    }
}
