namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202507040803)]
    public class FixCreateOrAlterGetSelfAssessmentReport : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_5759_CreateOrAlterSelfAssessmentReportSPandTVF_Fix_UP);
        }
    }
}
