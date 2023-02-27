namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202202140841)]
    public class AddReviewerCommentsFieldToSelfassessment : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments")
                .AddColumn("ReviewerCommentsLabel").AsString(50).Nullable();
        }

        public override void Down()
        {
            Delete.Column("ReviewerCommentsLabel").FromTable("SelfAssessments");
        }
    }
}
