namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202008100854)]
    public class AddCandidateAssessmentNewFields : Migration
    {
        public override void Up()
        {
            Alter.Table("CandidateAssessments")
                .AddColumn("UserBookmark").AsString(100).Nullable()
                .AddColumn("UnprocessedUpdates").AsBoolean().NotNullable().WithDefaultValue(true);
        }

        public override void Down()
        {
            Delete.Column("UserBookmark").FromTable("CandidateAssessments");
            Delete.Column("UnprocessedUpdates").FromTable("CandidateAssessments");
        }
    }
}
