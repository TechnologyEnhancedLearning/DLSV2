namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202205051500)]

    public class AddManageSupervisorsDescriptionToSelfAssessments : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("ManageSupervisorsDescription").AsString(1000).Nullable();
        }

        public override void Down()
        {
            Delete.Column("ManageSupervisorsDescription").FromTable("SelfAssessments");
        }
    }

}
