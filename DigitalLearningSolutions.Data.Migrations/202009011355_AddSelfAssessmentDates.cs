namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202009011355)]
    public class AddSelfAssessmentDates : Migration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessments")
                .AddColumn("StartedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .AddColumn("LastAccessed").AsDateTime().Nullable()
                .AddColumn("CompleteByDate").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Column("CompleteByDate").FromTable("CandidateAssessments");
            Delete.Column("LastAccessed").FromTable("CandidateAssessments");
            Delete.Column("StartedDate").FromTable("CandidateAssessments");
        }
    }
}
