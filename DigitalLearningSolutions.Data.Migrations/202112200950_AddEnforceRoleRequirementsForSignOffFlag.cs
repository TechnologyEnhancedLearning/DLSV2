namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202112200950)]
    public class AddEnforceRoleRequirementsForSignOffFlag : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("EnforceRoleRequirementsForSignOff").AsBoolean()
                .WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("EnforceRoleRequirementsForSignOff").FromTable("SelfAssessments");
        }
    }
}
