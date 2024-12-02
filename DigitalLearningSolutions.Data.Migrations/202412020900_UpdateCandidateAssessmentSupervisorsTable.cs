namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202412020900)]
    public class UpdateCandidateAssessmentSupervisorsTable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql($@"UPDATE cas 
                SET SelfAssessmentSupervisorRoleID = (SELECT ID FROM SelfAssessmentSupervisorRoles 
								        WHERE SelfAssessmentID = ssr.SelfAssessmentID and AllowDelegateNomination = 1) 
                FROM CandidateAssessmentSupervisors cas INNER JOIN 
                    SelfAssessmentSupervisorRoles ssr ON cas.SelfAssessmentSupervisorRoleID = ssr.ID 
                    AND cas.Removed IS NULL AND ssr.AllowDelegateNomination = 0 INNER JOIN 
                    SupervisorDelegates sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN 
                    AdminAccounts aa ON sd.SupervisorAdminID = aa.ID 
		        WHERE aa.IsSupervisor = 0 AND aa.IsNominatedSupervisor = 1
				        -- to exclude duplicate CandidateAssessmentID from update
				        AND cas.CandidateAssessmentID NOT IN (
					        SELECT CandidateAssessmentID FROM CandidateAssessmentSupervisors WHERE CandidateAssessmentID in (
						        SELECT Cas2.CandidateAssessmentID
						        FROM CandidateAssessmentSupervisors cas2 with (nolock) INNER JOIN 
							        SelfAssessmentSupervisorRoles ssr2 with (nolock) ON cas2.SelfAssessmentSupervisorRoleID = ssr2.ID 
							        AND cas2.Removed IS NULL AND ssr2.AllowDelegateNomination = 0 INNER JOIN
							        SupervisorDelegates sd2 with (nolock) ON cas2.SupervisorDelegateId = sd2.ID INNER JOIN
							        AdminAccounts aa2 with (nolock) ON sd2.SupervisorAdminID = aa2.ID
						        WHERE aa2.IsSupervisor = 0 AND aa2.IsNominatedSupervisor = 1
					        )
					        GROUP BY CandidateAssessmentID,SupervisorDelegateId
					        HAVING COUNT(*)>1
					        )"
            );
        }
    }
}
