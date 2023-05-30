using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202305250945)]
    public class AddAllowSupervisorRoleSelectionColumnToSelfAssessmentSupervisorRoles : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentSupervisorRoles").AddColumn("AllowSupervisorRoleSelection").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(
                @$"UPDATE SelfAssessmentSupervisorRoles SET AllowDelegateNomination = 1, AllowSupervisorRoleSelection = 0
                    WHERE RoleName = 'Assessor';"
            );
            Execute.Sql(
                @$"UPDATE SelfAssessmentSupervisorRoles SET AllowDelegateNomination = 0, AllowSupervisorRoleSelection = 1
                    WHERE RoleName = 'Educator/Manager';"
            );
        }
        public override void Down()
        {
            Delete.Column("AllowSupervisorRoleSelection").FromTable("SelfAssessmentSupervisorRoles");
        }
    }
}

