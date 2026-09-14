namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202608122218)]
    public class Alter_GetAssessmentResultByDelegate : Migration
    {
        public override void Up()
        {
            Create.Index("IX_SelfAssessmentResults_Assessment_Delegate_Question")
                .OnTable("SelfAssessmentResults")
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("DelegateUserID").Ascending()
                .OnColumn("CompetencyID").Ascending()
                .OnColumn("AssessmentQuestionID").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("ID")
                .Include("DateTime")
                .Include("Result")
                .Include("SupportingComments");

            Create.Index("IX_CandidateAssessments_Active_Assessment_Delegate")
                .OnTable("CandidateAssessments")
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("DelegateUserID").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("ID");

            //Execute.Sql(@"CREATE NONCLUSTERED INDEX IX_CandidateAssessments_Active_Assessment_Delegate ON CandidateAssessments (SelfAssessmentID, DelegateUserID) INCLUDE (ID) WHERE RemovedDate IS NULL;");

            Create.Index("IX_SelfAssessmentStructure_Assessment_Competency")
                .OnTable("SelfAssessmentStructure")
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("CompetencyID").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("CompetencyGroupID")
                .Include("Ordering")
                .Include("Optional");

            Create.Index("IX_CompetencyAssessmentQuestions_Competency_Question")
                .OnTable("CompetencyAssessmentQuestions")
                .OnColumn("CompetencyID").Ascending()
                .OnColumn("AssessmentQuestionID").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("Ordering")
                .Include("Required");

            Create.Index("IX_CandidateAssessmentOptionalCompetencies_Lookup")
                .OnTable("CandidateAssessmentOptionalCompetencies")
                .OnColumn("CandidateAssessmentID").Ascending()
                .OnColumn("CompetencyID").Ascending()
                .OnColumn("CompetencyGroupID").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("IncludedInSelfAssessment");

            Create.Index("IX_SelfAssessmentResultSupervisorVerifications_Result_Active")
                .OnTable("SelfAssessmentResultSupervisorVerifications")
                .OnColumn("SelfAssessmentResultId").Ascending()
                .OnColumn("Superceded").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("ID")
                .Include("Requested")
                .Include("Verified")
                .Include("Comments")
                .Include("SignedOff")
                .Include("CandidateAssessmentSupervisorID");

            Create.Index("IX_CompetencyAssessmentQuestionRoleRequirements_ResultRag")
                .OnTable("CompetencyAssessmentQuestionRoleRequirements")
                .OnColumn("SelfAssessmentID").Ascending()
                .OnColumn("CompetencyID").Ascending()
                .OnColumn("AssessmentQuestionID").Ascending()
                .OnColumn("LevelValue").Ascending()
                .WithOptions()
                .NonClustered()
                .Include("LevelRAG");

            Create.Index("IX_FrameworkCompetencies_Competency_Framework")
                .OnTable("FrameworkCompetencies")
                .OnColumn("CompetencyID").Ascending()
                .OnColumn("FrameworkID").Ascending()
                .WithOptions()
                .NonClustered();

            Execute.Sql(Properties.Resources.TD_7590_Alter_GetAssessmentResultByDelegate_Up);
        }
        public override void Down()
        {
            Delete.Index("IX_SelfAssessmentResults_Assessment_Delegate_Question")
                .OnTable("SelfAssessmentResults");

            Delete.Index("IX_CandidateAssessments_Active_Assessment_Delegate")
                .OnTable("CandidateAssessments");

            Delete.Index("IX_SelfAssessmentStructure_Assessment_Competency")
                .OnTable("SelfAssessmentStructure");

            Delete.Index("IX_CompetencyAssessmentQuestions_Competency_Question")
                .OnTable("CompetencyAssessmentQuestions");

            Delete.Index("IX_CandidateAssessmentOptionalCompetencies_Lookup")
                .OnTable("CandidateAssessmentOptionalCompetencies");

            Delete.Index("IX_SelfAssessmentResultSupervisorVerifications_Result_Active")
                .OnTable("SelfAssessmentResultSupervisorVerifications");

            Delete.Index("IX_CompetencyAssessmentQuestionRoleRequirements_ResultRag")
                .OnTable("CompetencyAssessmentQuestionRoleRequirements");

            Delete.Index("IX_FrameworkCompetencies_Competency_Framework")
                .OnTable("FrameworkCompetencies");

            Execute.Sql(Properties.Resources.TD_7590_Alter_GetAssessmentResultByDelegate_Down);
        }
    }
}
