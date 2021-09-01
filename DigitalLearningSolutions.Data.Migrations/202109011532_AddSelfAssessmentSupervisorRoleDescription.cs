namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109011532)]
    public class AddSelfAssessmentSupervisorRoleDescription : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessmentSupervisorRoles").AddColumn("RoleDescription").AsString(500).Nullable();
        }
        public override void Down()
        {
            Delete.Column("RoleDescription").FromTable("SelfAssessmentSupervisorRoles");
        }
    }
}
