namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202409300912)]
    public class AlterConstraintForCandidateAssessmentsAddUtcDate : Migration
    {
        public override void Up()
        {
            Execute.Sql(@$"ALTER TABLE [dbo].[CandidateAssessments] DROP CONSTRAINT [DF_CandidateAssessments_StartedDate];
                           ALTER TABLE [dbo].[CandidateAssessments] ADD CONSTRAINT [DF_CandidateAssessments_StartedDate] DEFAULT (GETUTCDATE()) FOR [StartedDate];");
        }
        public override void Down()
        {
            Execute.Sql(@$"ALTER TABLE [dbo].[CandidateAssessments] DROP CONSTRAINT [DF_CandidateAssessments_StartedDate];
                           ALTER TABLE [dbo].[CandidateAssessments] ADD CONSTRAINT [DF_CandidateAssessments_StartedDate] DEFAULT (GETDATE()) FOR [StartedDate];");
        }

    }
}
