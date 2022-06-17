namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202204071000)]
    public class AddDetailsLastCheckedToUsersTableAndRenameOnDelegateAccounts : Migration
    {
        public override void Up()
        {
            Alter.Table("Users").AddColumn("DetailsLastChecked").AsDateTime().Nullable();
            Rename.Column("DetailsLastChecked").OnTable("DelegateAccounts").To("CentreSpecificDetailsLastChecked");
            Delete.Index("IX_AdminAccounts_Email").OnTable("AdminAccounts");
        }

        public override void Down()
        {
            Rename.Column("CentreSpecificDetailsLastChecked").OnTable("DelegateAccounts").To("DetailsLastChecked");
            Delete.Column("DetailsLastChecked").FromTable("Users");
            Create.Index("IX_AdminAccounts_Email").OnTable("AdminAccounts").OnColumn("Email").Ascending()
                .WithOptions().Unique().WithOptions().NonClustered();
        }
    }
}
