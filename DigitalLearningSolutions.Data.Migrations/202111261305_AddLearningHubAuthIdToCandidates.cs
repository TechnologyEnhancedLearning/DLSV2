namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202111261305)]
    public class AddLearningHubAuthIdToCandidates : Migration
    {
        public override void Up()
        {
            Alter.Table("Candidates")
                .AddColumn("LearningHubAuthId").AsInt32().Nullable();
        }

        public override void Down()
        {
            Delete.Column("LearningHubAuthId").FromTable("Candidates");
        }
    }
}
