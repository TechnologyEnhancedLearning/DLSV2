using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202302221309)]
    public class AlterViewCandidatesAddUserID : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.td_1131_alterviewcandidatesadduserid_up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.td_1131_alterviewcandidatesadduserid_down);
        }
    }
}
