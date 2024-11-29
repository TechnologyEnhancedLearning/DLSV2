

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202411120903)]
    public class CreateOrAlterGetOtherCentresForSelfAssessmentFunction : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_4950_dboGetOtherCentresForSelfAssessmentCreateOrAlter);
        }
    }
}
