namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109161053)]
    public class AddsCandidateAssessmentOptionalCompetenciesTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("CandidateAssessmentOptionalCompetencies")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CandidateAssessmentID").AsInt32().NotNullable().ForeignKey("CandidateAssessments", "ID")
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("IncludedInSelfAssessment").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
