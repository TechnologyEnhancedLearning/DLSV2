namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202107221605)]
    public class ChangeEmailMaxLengthOnAdminUsers : Migration
    {
        public override void Up()
        {
            Alter.Column("Email").OnTable("AdminUsers").AsString(255).NotNullable();
        }
        public override void Down()
        {
            Alter.Column("Email").OnTable("AdminUsers").AsString(250).NotNullable();
        }
    }
}
