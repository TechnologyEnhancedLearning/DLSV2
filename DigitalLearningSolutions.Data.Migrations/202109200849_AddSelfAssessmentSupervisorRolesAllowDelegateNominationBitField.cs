namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109200849)]
    public class AddSelfAssessmentSupervisorRolesAllowDelegateNominationBitField : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentSupervisorRoles").AddColumn("AllowDelegateNomination").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
