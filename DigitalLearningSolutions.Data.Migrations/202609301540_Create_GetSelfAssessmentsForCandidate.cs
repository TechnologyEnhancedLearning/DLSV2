using FluentMigrator;
using System;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202609301540)]
    public class Create_GetSelfAssessmentsForCandidate : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.TD_7820_CreateSP_GetSelfAssessmentsForCandidate);
            Create.Index("IX_SelfAssessmentStructure_SelfAssessmentID_CompetencyID")
               .OnTable("SelfAssessmentStructure")
               .OnColumn("SelfAssessmentID").Ascending()
               .OnColumn("CompetencyID").Ascending();
        }
        public override void Down()
        {
            Execute.Sql(@"DROP PROCEDURE [dbo].[GetSelfAssessmentsForCandidate]");
            Delete.Index("IX_SelfAssessmentStructure_SelfAssessmentID_CompetencyID")
                .OnTable("SelfAssessmentStructure");
        }
    }
}
