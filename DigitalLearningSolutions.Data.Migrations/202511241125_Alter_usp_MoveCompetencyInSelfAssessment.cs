

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202511241125)]
    public class Alter_usp_MoveCompetencyInSelfAssessment : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_483_Alter_usp_MoveCompetencyInSelfAssessment_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_483_Alter_usp_MoveCompetencyInSelfAssessment_Down);
        }
    }
}
