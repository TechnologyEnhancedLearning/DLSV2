namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202107221605)]
    public class ChangeEmailMaxLengthOnAdminUsers : Migration
    {
        public override void Up()
        {
            Delete.Index("IX_AdminUsers_Email");
            Alter.Column("Email").OnTable("AdminUsers").AsString(255);
            Create.Index("IX_AdminUsers_Email").OnTable("AdminUsers").OnColumn("Email").Ascending().WithOptions()
                .Unique().WithOptions().NonClustered();
        }

        public override void Down()
        {
            Delete.Index("IX_AdminUsers_Email");
            Alter.Column("Email").OnTable("AdminUsers").AsString(250);
            Create.Index("IX_AdminUsers_Email").OnTable("AdminUsers").OnColumn("Email").Ascending().WithOptions()
                .Unique().WithOptions().NonClustered();
            ;
        }
    }
}
