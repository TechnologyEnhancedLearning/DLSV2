namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202104271615)]
    public class AddEmailToFrameworkCollaboratorTable : Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK_FrameworkCollaborators").FromTable("FrameworkCollaborators");
            Alter.Table("FrameworkCollaborators").AddColumn("ID").AsInt32().NotNullable().Identity();
            Create.PrimaryKey("PK_FrameworkCollaborators").OnTable("FrameworkCollaborators").Column("ID");
            Alter.Table("FrameworkCollaborators").AddColumn("UserEmail").AsString(255).Nullable();
            Execute.Sql("UPDATE FrameworkCollaborators SET UserEmail = (SELECT Email FROM AdminUsers WHERE AdminID = FrameworkCollaborators.AdminID);");
            Alter.Column("UserEmail").OnTable("FrameworkCollaborators").AsString(255).NotNullable();
            Alter.Column("AdminID").OnTable("FrameworkCollaborators").AsInt32().Nullable();
            Alter.Table("AdminUsers").AddColumn("IsFrameworkContributor").AsBoolean().NotNullable().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Column("UserEmail").FromTable("FrameworkCollaborators");
            Execute.Sql("DELETE FROM FrameworkCollaborators WHERE AdminID IS NULL;");
            Alter.Column("AdminID").OnTable("FrameworkCollaborators").AsInt32().NotNullable();
            Delete.PrimaryKey("PK_FrameworkCollaborators").FromTable("FrameworkCollaborators");
            Delete.Column("ID").FromTable("FrameworkCollaborators");
            Create.PrimaryKey("PK_FrameworkCollaborators").OnTable("FrameworkCollaborators").Columns("AdminID", "FrameworkID");
            Delete.Column("IsFrameworkContributor").FromTable("AdminUsers");
        }
    }
}
