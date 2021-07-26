namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107191420)]
    public class CandidateSelfAssessmentSupervisorRoleLookup : Migration
    {
        public override void Up()
        {
            Delete.Column("SupervisorRoleTitle").FromTable("CandidateAssessmentSupervisors");
            Alter.Table("CandidateAssessmentSupervisors").AddColumn("SelfAssessmentSupervisorRoleID").AsInt32().Nullable().ForeignKey("SelfAssessmentSupervisorRoles", "ID");
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_CandidateAssessmentSupervisors_SelfAssessmentSupervisorRoleID_SelfAssessmentSupervisorRoles_ID").OnTable("CandidateAssessmentSupervisors");
            Delete.Column("SelfAssessmentSupervisorRoleID").FromTable("CandidateAssessmentSupervisors");
            Alter.Table("CandidateAssessmentSupervisors").AddColumn("SupervisorRoleTitle").AsString(100).Nullable();
        }
    }
}
