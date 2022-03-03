namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202202281322)]
    public class AddNominatedSupervisorToAdminUsersTable : Migration
    {
        public override void Up()
        {
            Alter.Table("AdminUsers").AddColumn("NominatedSupervisor").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("NominatedSupervisor").FromTable("AdminUsers");
        }
    }
}
