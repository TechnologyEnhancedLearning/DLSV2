using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202303071200)]
    public class AddUniqueConstraintToCandidateAssessmentSupervisors : Migration
    {
        public override void Down()
        {
            Delete.UniqueConstraint(
                "IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID"
            )
            .FromTable("CandidateAssessmentSupervisors");
        }

        public override void Up()
        {
            Execute.Sql(@"DELETE CAS
                          FROM CandidateAssessmentSupervisors CAS
                          INNER JOIN
                          (
                            SELECT *, 
                            RANK() OVER(PARTITION BY CandidateAssessmentID, 
                                        SupervisorDelegateId, 
                                        SelfAssessmentSupervisorRoleID
                            ORDER BY id) rank
                            FROM CandidateAssessmentSupervisors
                           ) T ON CAS.ID = T.ID
                        WHERE rank > 1");
            Create.UniqueConstraint(
                    "IX_CandidateAssessmentSupervisors_CandidateAssessmentID_SupervisorDelegateId_SelfAssessmentSupervisorRoleID"
                )
                .OnTable("CandidateAssessmentSupervisors").Columns(
                    "CandidateAssessmentID",
                    "SupervisorDelegateId",
                    "SelfAssessmentSupervisorRoleID"
                );
        }
    }
}
