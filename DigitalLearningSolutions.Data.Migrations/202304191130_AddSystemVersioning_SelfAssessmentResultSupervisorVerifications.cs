namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202304191130)]
    public class AddSystemVersioning_SelfAssessmentResultSupervisorVerifications : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_1220_AddSystemVersioning_SelfAssessmentResultSupervisorVerifications);
        }

        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_1220_RemoveSystemVersioning_SelfAssessmentResultSupervisorVerifications);
        }
    }
}
