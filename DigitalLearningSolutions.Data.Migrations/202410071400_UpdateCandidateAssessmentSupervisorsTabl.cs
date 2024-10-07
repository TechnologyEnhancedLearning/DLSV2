
namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202410071400)]
    public class UpdateCandidateAssessmentSupervisorsTabl : Migration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE cas " +
                "SET SelfAssessmentSupervisorRoleID = (SELECT ID FROM SelfAssessmentSupervisorRoles " +
                    "WHERE SelfAssessmentID = ssr.SelfAssessmentID and AllowDelegateNomination = 1) " +
                "FROM CandidateAssessmentSupervisors cas INNER JOIN " +
                    "SelfAssessmentSupervisorRoles ssr ON cas.SelfAssessmentSupervisorRoleID = ssr.ID " +
                    "AND cas.Removed IS NULL AND ssr.AllowDelegateNomination = 0 INNER JOIN " +
                    "SupervisorDelegates sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN " +
                    "AdminAccounts aa ON sd.SupervisorAdminID = aa.ID WHERE aa.IsSupervisor = 0 AND aa.IsNominatedSupervisor = 1");
        }

        public override void Down()
        {

        }
    }
}
