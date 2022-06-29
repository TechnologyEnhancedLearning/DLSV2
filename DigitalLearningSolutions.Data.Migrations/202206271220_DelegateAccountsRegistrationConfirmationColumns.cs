namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202206271220)]
    public class DelegateAccountsRegistrationConfirmationColumns : Migration
    {
        public override void Up()
        {
            Alter.Table("DelegateAccounts")
                .AddColumn("RegistrationConfirmationHashCreationDateTime").AsDateTime().Nullable();
            Alter.Table("DelegateAccounts")
                .AddColumn("RegistrationConfirmationHash").AsString(64).Nullable();
        }

        public override void Down()
        {
            Delete.Column("RegistrationConfirmationHashCreationDateTime").FromTable("DelegateAccounts");
            Delete.Column("RegistrationConfirmationHash").FromTable("DelegateAccounts");
        }
    }
}
