namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202104271615)]
    public class AddEmailToFrameworkCollaboratorTable : Migration
    {
        public override void Up()
        {
            Alter.Table("FrameworkCollaborators").AddColumn("UserEmail").AsString(255).Nullable();
            Execute.Sql("UPDATE FrameworkCollaborators SET UserEmail = (SELECT Email FROM AdminUsers WHERE AdminID = FrameworkCollaborators.AdminID);");
            Alter.Column("UserEmail").OnTable("FrameworkCollaborators").AsString(255).NotNullable();
            Alter.Column("AdminID").OnTable("FrameworkCollaborators").AsInt32().Nullable();
        }
        public override void Down()
        {
            Delete.Column("UserEmail").FromTable("FrameworkCollaborators");
            Execute.Sql("DELETE FROM FrameworkCollaborators WHERE AdminID IS NULL;");
            Alter.Column("AdminID").OnTable("FrameworkCollaborators").AsInt32().NotNullable();
        }
    }
}
