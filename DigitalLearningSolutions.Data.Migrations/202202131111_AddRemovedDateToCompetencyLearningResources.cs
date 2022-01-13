namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202202131111)]
    public class AddRemovedDateToCompetencyLearningResources : Migration
    {
        public override void Up()
        {
            Alter.Table("CompetencyLearningResources").AddColumn("RemovedDate").AsDateTime().Nullable();
        }
        public override void Down()
        {
            Delete.Column("RemovedDate").FromTable("CompetencyLearningResources");
        }
    }
}
