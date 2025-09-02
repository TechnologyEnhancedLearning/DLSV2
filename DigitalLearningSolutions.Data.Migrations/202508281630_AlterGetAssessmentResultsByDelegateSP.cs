

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202508281630)]
    public class AlterGetAssessmentResultsByDelegateSP : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5638_Alter_GetAssessmentResultsByDelegate_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_5638_Alter_GetAssessmentResultsByDelegate_DOWN);
        }
    }
}
