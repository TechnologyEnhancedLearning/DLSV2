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
            Execute.Sql(@" WITH CTEUpdate AS (SELECT CASV.ID,CASV.CandidateAssessmentID, CASV.SupervisorDelegateId,CASV.SelfAssessmentSupervisorRoleID,CASVTemp.ID AS Original_CASVId
                           FROM CandidateAssessmentSupervisors CASV
                           INNER JOIN
                            (
                             SELECT MIN(ID) AS ID, CandidateAssessmentID,SelfAssessmentSupervisorRoleID,SupervisorDelegateId 
                             FROM CandidateAssessmentSupervisors
                             GROUP BY CandidateAssessmentID,SelfAssessmentSupervisorRoleID,SupervisorDelegateId 
                             HAVING COUNT(id) > 1
                            ) CASVTemp 
                           ON CASV.ID > CASVTemp.ID AND CASV.CandidateAssessmentID=CASVTemp.CandidateAssessmentID 
                               AND CASV.SelfAssessmentSupervisorRoleID=CASVTemp.SelfAssessmentSupervisorRoleID 
                               AND CASV.SupervisorDelegateId=CASVTemp.SupervisorDelegateId) 
                           UPDATE SelfAssessmentResultSupervisorVerifications
                                 SET CandidateAssessmentSupervisorID = C.Original_CASVId     					
                           FROM SelfAssessmentResultSupervisorVerifications CASV
                           INNER JOIN CTEUpdate C
						   ON CASV.CandidateAssessmentSupervisorID=C.ID ");
            Execute.Sql(@" WITH CTEUpdate AS (SELECT CASV.ID,CASV.CandidateAssessmentID, CASV.SupervisorDelegateId,CASV.SelfAssessmentSupervisorRoleID,CASVTemp.ID AS Original_CASVId
                           FROM CandidateAssessmentSupervisors CASV
                           INNER JOIN
                            (
                             SELECT MIN(ID) AS ID, CandidateAssessmentID,SelfAssessmentSupervisorRoleID,SupervisorDelegateId 
                             FROM CandidateAssessmentSupervisors
                             GROUP BY CandidateAssessmentID,SelfAssessmentSupervisorRoleID,SupervisorDelegateId 
                             HAVING COUNT(id) > 1
                            ) CASVTemp 
                           ON CASV.ID > CASVTemp.ID AND CASV.CandidateAssessmentID=CASVTemp.CandidateAssessmentID 
                               AND CASV.SelfAssessmentSupervisorRoleID=CASVTemp.SelfAssessmentSupervisorRoleID 
                               AND CASV.SupervisorDelegateId=CASVTemp.SupervisorDelegateId) 
                           UPDATE CandidateAssessmentSupervisorVerifications
                              SET CandidateAssessmentSupervisorID = C.Original_CASVId     					
                           FROM CandidateAssessmentSupervisorVerifications CASV
                           INNER JOIN CTEUpdate C
						   ON CASV.CandidateAssessmentSupervisorID=C.ID ");
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
