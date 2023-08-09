using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202306161531)]
    public class AlterAddCourseToGroupSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_1913_AlterGroupCustomisation_Add_V2_UP);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_1913_AlterGroupCustomisation_Add_V2_DOWN);
        }
    }
}
