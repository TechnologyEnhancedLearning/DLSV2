using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202305221300)]
    public class AddUpdateBitColumnsToSelfAssessmentSupervisorRolesTable : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentSupervisorRoles").AddColumn("AllowLearnerRoleSelection").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("SelfAssessmentSupervisorRoles").AddColumn("AllowSupervisorRoleSelection").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(
                @$"UPDATE SelfAssessmentSupervisorRoles SET allowLearnerRoleSelection = 1, AllowSupervisorRoleSelection = 0 WHERE RoleName = 'Assessor';
                   UPDATE SelfAssessmentSupervisorRoles SET allowLearnerRoleSelection = 0, AllowSupervisorRoleSelection = 1 WHERE RoleName = 'Educator/Manager'"
            );
        }
        public override void Down()
        {
            Delete.Column("AllowLearnerRoleSelection").FromTable("SelfAssessmentSupervisorRoles");
            Delete.Column("AllowSupervisorRoleSelection").FromTable("SelfAssessmentSupervisorRoles");
        }
    }
}

