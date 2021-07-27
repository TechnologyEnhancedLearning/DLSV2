namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202107221606)]
    public class ChangeEmailMaxLengthOnAdminUsers : Migration
    {
        public override void Up()
        {
            Delete.Index("IX_AdminUsers_Email").OnTable("AdminUsers");
            Alter.Column("Email").OnTable("AdminUsers").AsString(255).Nullable();
            Create.Index("IX_AdminUsers_Email").OnTable("AdminUsers").OnColumn("Email").Ascending().WithOptions()
                .Unique().WithOptions().NonClustered();
        }

        public override void Down()
        {
            Delete.Index("IX_AdminUsers_Email").OnTable("AdminUsers");
            Alter.Column("Email").OnTable("AdminUsers").AsString(250).Nullable();
            Create.Index("IX_AdminUsers_Email").OnTable("AdminUsers").OnColumn("Email").Ascending().WithOptions()
                .Unique().WithOptions().NonClustered();
        }
    }
}
