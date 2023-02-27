namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202012151501)]
    public class AddFrameworkIdToFrameworkCompetencies : Migration
    {
        public override void Up()
        {
            Alter.Table("FrameworkCompetencies").AddColumn("FrameworkID").AsInt32().NotNullable().ForeignKey("Frameworks", "ID");
            Alter.Table("SelfAssessmentStructure").AddColumn("CompetencyGroupID").AsInt32().Nullable().ForeignKey("CompetencyGroups", "ID");
            Execute.Sql("UPDATE SelfAssessmentStructure SET CompetencyGroupID = c.CompetencyGroupID FROM SelfAssessmentStructure INNER JOIN Competencies AS c ON SelfAssessmentStructure.CompetencyID = c.ID");
            Delete.ForeignKey("FK_Competencies_CompetencyGroupID_CompetencyGroups_ID").OnTable("Competencies");
            Delete.Column("CompetencyGroupID").FromTable("Competencies");
        }

        public override void Down()
        {
            Delete.Column("FrameworkID").FromTable("FrameworkCompetencies");
            Alter.Table("Competencies").AddColumn("CompetencyGroupID").AsInt32().Nullable().ForeignKey("CompetencyGroups", "ID");
            Execute.Sql("UPDATE Competencies SET CompetencyGroupID = sas.CompetencyGroupID FROM SelfAssessmentStructure AS sas INNER JOIN Competencies ON sas.CompetencyID = Competencies.ID");
            Delete.ForeignKey("FK_SelfAssessmentStructure_CompetencyGroupID_CompetencyGroups_ID").OnTable("SelfAssessmentStructure");
            Delete.Column("CompetencyGroupID").FromTable("SelfAssessmentStructure");
        }
    }
}
