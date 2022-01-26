namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202202131112)]
    public class AddRemovedDateToCompetencyLearningResources : Migration
    {
        public override void Up()
        {
            Alter.Table("CompetencyLearningResources").AddColumn("RemovedDate").AsDateTime().Nullable();
            Alter.Table("CompetencyLearningResources").AddColumn("RemovedByAdminID").AsInt32().Nullable();
        }
        public override void Down()
        {
            Delete.Column("RemovedDate").FromTable("CompetencyLearningResources");
            Delete.Column("RemovedByAdminID").FromTable("CompetencyLearningResources");
        }
    }
}
