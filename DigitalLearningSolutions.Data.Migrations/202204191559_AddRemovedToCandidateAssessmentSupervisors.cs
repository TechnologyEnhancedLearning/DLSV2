namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202204191559)]
    public class AddRemovedCandidateAssessmentSupervisors : Migration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessmentSupervisors").AddColumn("Removed").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Column("Removed").FromTable("CandidateAssessmentSupervisors");
        }
    }
}
