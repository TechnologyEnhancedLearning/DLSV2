namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202103050844)]
    public class AddAssessAttemptsIndex : Migration
    {
        public override void Up()
        {
            Create.Index("IX_AssessAttempts_SectionNumber_ProgressID").OnTable("AssessAttempts").OnColumn("SectionNumber").Ascending().OnColumn("ProgressID").Ascending().WithOptions().NonClustered();
        }

        public override void Down()
        {
            Delete.Index("IX_AssessAttempts_SectionNumber_ProgressID").OnTable("AssessAttempts");
        }
    }
}
