namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202607301180)]
    public class TD_7501_Alter_GetAssessmentResultsByDelegate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7501_Alter_GetAssessmentResultsByDelegate_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7501_Alter_GetAssessmentResultsByDelegate_Down);
        }
    }
}
