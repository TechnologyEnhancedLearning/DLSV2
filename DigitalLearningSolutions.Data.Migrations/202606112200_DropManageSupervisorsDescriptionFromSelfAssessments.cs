namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202606112200)]
    public class DropManageSupervisorsDescriptionFromSelfAssessments : Migration
    {
        public override void Up()
        {
            Delete.Column("ManageSupervisorsDescription").FromTable("SelfAssessments");
        }

        public override void Down()
        {
            Alter.Table("SelfAssessments").AddColumn("ManageSupervisorsDescription").AsString(1000).Nullable();
        }
    }
}
