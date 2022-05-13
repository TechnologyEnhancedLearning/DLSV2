namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202205061441)]
    public class SeparateCentreEmailsTable : Migration
    {
        public override void Up()
        {
            Create.Table("UserCentreDetails")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("UserID").AsInt32().NotNullable().ForeignKey("Users", "ID")
                .WithColumn("CentreID").AsInt32().NotNullable().ForeignKey("Centres", "CentreId")
                .WithColumn("Email").AsString(255).Nullable()
                .WithColumn("EmailVerified").AsDateTime().Nullable();

            Delete.Index("IX_AdminAccounts_CentreId_Email").OnTable("AdminAccounts");
            Delete.Index("IX_DelegateAccounts_CentreId_Email").OnTable("DelegateAccounts");

            Rename.Column("Email").OnTable("AdminAccounts").To("Email_deprecated");
            Rename.Column("Email").OnTable("DelegateAccounts").To("Email_deprecated");

            Execute.Sql(
                @"CREATE UNIQUE NONCLUSTERED INDEX IX_UserCentreDetails_CentreId_Email
                ON UserCentreDetails (CentreId, Email)
                WHERE Email IS NOT NULL"
            );

            Create.UniqueConstraint("IX_UserCentreDetails_UserId_CentreId").OnTable("UserCentreDetails")
                .Columns("UserId", "CentreId");

            Delete.Column("EmailVerified").FromTable("AdminAccounts");
            Delete.Column("EmailVerified").FromTable("DelegateAccounts");
        }

        public override void Down()
        {
            Delete.Table("UserCentreDetails");

            Rename.Column("Email_deprecated").OnTable("AdminAccounts").To("Email");
            Rename.Column("Email_deprecated").OnTable("DelegateAccounts").To("Email");

            Execute.Sql(
                @"CREATE UNIQUE NONCLUSTERED INDEX IX_DelegateAccounts_CentreId_Email
                ON DelegateAccounts (CentreId, Email)
                WHERE Email IS NOT NULL"
            );

            Execute.Sql(
                @"CREATE UNIQUE NONCLUSTERED INDEX IX_AdminAccounts_CentreId_Email
                ON AdminAccounts (CentreId, Email)
                WHERE Email IS NOT NULL"
            );

            Alter.Table("AdminAccounts").AddColumn("EmailVerified").AsDateTime().Nullable();
            Alter.Table("DelegateAccounts").AddColumn("EmailVerified").AsDateTime().Nullable();
        }
    }
}
