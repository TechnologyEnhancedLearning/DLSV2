

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202504281045)]
    public class Alter_ReorderFrameworkCompetency : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5447_Alter_ReorderFrameworkCompetency_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5447_Alter_ReorderFrameworkCompetency_Down);
        }
    }
}
