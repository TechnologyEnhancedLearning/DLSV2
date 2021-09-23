namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202109201529)]
    public class AddCompetencyGroupIdToCandidateAssessmentOptionalCompetencies : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessmentOptionalCompetencies").AddColumn("CompetencyGroupID").AsInt32().Nullable().ForeignKey("CompetencyGroups", "ID");
        }
    }
}
