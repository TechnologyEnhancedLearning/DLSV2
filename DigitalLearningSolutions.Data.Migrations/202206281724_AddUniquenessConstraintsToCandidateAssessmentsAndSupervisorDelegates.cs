namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202206281724)]
    public class AddUniquenessConstraintsToCandidateAssessmentsAndSupervisorDelegates : Migration
    {
        private const string CandidateAssessmentsIndexName = "IX_CandidateAssessments_DelegateUserId_SelfAssessmentId";
        private const string SupervisorDelegatesIndexName = "IX_SupervisorDelegates_DelegateUserId_SupervisorAdminId";
        private const string DeleteIllegalDuplicatesSql =
            @"DELETE FROM CandidateAssessmentOptionalCompetencies
FROM   CandidateAssessments AS CA INNER JOIN
             CandidateAssessmentOptionalCompetencies ON CA.ID = CandidateAssessmentOptionalCompetencies.CandidateAssessmentID
WHERE (CA.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    CandidateAssessments AS CA1
                 WHERE (CA.SelfAssessmentID = SelfAssessmentID) AND (CA.DelegateUserID = DelegateUserID)))

DELETE FROM CandidateAssessmentSupervisorVerifications 
FROM   CandidateAssessments INNER JOIN
             CandidateAssessmentSupervisors ON CandidateAssessments.ID = CandidateAssessmentSupervisors.CandidateAssessmentID INNER JOIN
             CandidateAssessmentSupervisorVerifications ON CandidateAssessmentSupervisors.ID = CandidateAssessmentSupervisorVerifications.CandidateAssessmentSupervisorID
WHERE (CandidateAssessments.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    CandidateAssessments AS CA1
                 WHERE (CandidateAssessments.SelfAssessmentID = SelfAssessmentID) AND (CandidateAssessments.DelegateUserID = DelegateUserID)))

DELETE FROM SelfAssessmentResultSupervisorVerifications
FROM   CandidateAssessments INNER JOIN
             CandidateAssessmentSupervisors ON CandidateAssessments.ID = CandidateAssessmentSupervisors.CandidateAssessmentID INNER JOIN
             SelfAssessmentResultSupervisorVerifications ON CandidateAssessmentSupervisors.ID = SelfAssessmentResultSupervisorVerifications.CandidateAssessmentSupervisorID
WHERE (CandidateAssessments.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    CandidateAssessments AS CA1
                 WHERE (CandidateAssessments.SelfAssessmentID = SelfAssessmentID) AND (CandidateAssessments.DelegateUserID = DelegateUserID)))

DELETE FROM CandidateAssessmentLearningLogItems
FROM   CandidateAssessments INNER JOIN
             CandidateAssessmentLearningLogItems ON CandidateAssessments.ID = CandidateAssessmentLearningLogItems.CandidateAssessmentID
WHERE (CandidateAssessments.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    CandidateAssessments AS CA1
                 WHERE (CandidateAssessments.SelfAssessmentID = SelfAssessmentID) AND (CandidateAssessments.DelegateUserID = DelegateUserID)))

DELETE FROM CandidateAssessmentSupervisors
FROM   CandidateAssessments INNER JOIN
             CandidateAssessmentSupervisors ON CandidateAssessments.ID = CandidateAssessmentSupervisors.CandidateAssessmentID
WHERE (CandidateAssessments.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    CandidateAssessments AS CA1
                 WHERE (CandidateAssessments.SelfAssessmentID = SelfAssessmentID) AND (CandidateAssessments.DelegateUserID = DelegateUserID)))

DELETE FROM CandidateAssessments
WHERE (ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    CandidateAssessments AS CA1
                 WHERE (CandidateAssessments.SelfAssessmentID = SelfAssessmentID) AND (CandidateAssessments.DelegateUserID = DelegateUserID)))

DELETE FROM SelfAssessmentResultSupervisorVerifications
FROM   SupervisorDelegates INNER JOIN
             CandidateAssessmentSupervisors ON SupervisorDelegates.ID = CandidateAssessmentSupervisors.SupervisorDelegateId INNER JOIN
             SelfAssessmentResultSupervisorVerifications ON CandidateAssessmentSupervisors.ID = SelfAssessmentResultSupervisorVerifications.CandidateAssessmentSupervisorID
WHERE (SupervisorDelegates.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    SupervisorDelegates AS SupervisorDelegates_1
                 WHERE (SupervisorDelegates.DelegateUserID = DelegateUserID) AND (SupervisorDelegates.SupervisorAdminID = SupervisorAdminID)))

DELETE FROM CandidateAssessmentSupervisorVerifications
FROM   SupervisorDelegates INNER JOIN
             CandidateAssessmentSupervisors ON SupervisorDelegates.ID = CandidateAssessmentSupervisors.SupervisorDelegateId INNER JOIN
             CandidateAssessmentSupervisorVerifications ON CandidateAssessmentSupervisors.ID = CandidateAssessmentSupervisorVerifications.CandidateAssessmentSupervisorID
WHERE (SupervisorDelegates.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    SupervisorDelegates AS SupervisorDelegates_1
                 WHERE (SupervisorDelegates.DelegateUserID = DelegateUserID) AND (SupervisorDelegates.SupervisorAdminID = SupervisorAdminID)))

DELETE FROM CandidateAssessmentSupervisors
FROM   SupervisorDelegates INNER JOIN
             CandidateAssessmentSupervisors ON SupervisorDelegates.ID = CandidateAssessmentSupervisors.SupervisorDelegateId
WHERE (SupervisorDelegates.ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    SupervisorDelegates AS SupervisorDelegates_1
                 WHERE (SupervisorDelegates.DelegateUserID = DelegateUserID) AND (SupervisorDelegates.SupervisorAdminID = SupervisorAdminID)))



DELETE FROM SupervisorDelegates
WHERE (ID <
                 (SELECT MAX(ID) AS Expr1
                 FROM    SupervisorDelegates AS SupervisorDelegates_1
                 WHERE (SupervisorDelegates.DelegateUserID = DelegateUserID) AND (SupervisorDelegates.SupervisorAdminID = SupervisorAdminID)))";

        public override void Up()
        {
            Execute.Sql(DeleteIllegalDuplicatesSql);
            Create.Index(CandidateAssessmentsIndexName)
                .OnTable("CandidateAssessments").WithOptions().UniqueNullsNotDistinct()
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("DelegateUserID").Ascending();
            Create.Index(SupervisorDelegatesIndexName)
                .OnTable("SupervisorDelegates").WithOptions().UniqueNullsNotDistinct()
                .OnColumn("SupervisorAdminID").Ascending()
                .OnColumn("DelegateUserID").Ascending();
        }

        public override void Down()
        {
            Delete.Index(CandidateAssessmentsIndexName).OnTable("CandidateAssessments");
            Delete.Index(SupervisorDelegatesIndexName).OnTable("SupervisorDelegates");
        }
    }
}
