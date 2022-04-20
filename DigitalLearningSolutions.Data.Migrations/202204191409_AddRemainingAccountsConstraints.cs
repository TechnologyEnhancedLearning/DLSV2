namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202204191409)]
    public class AddRemainingAccountsConstraints : Migration
    {
        public override void Up()
        {
            Alter.Column("UserId").OnTable("AdminAccounts").AsInt32().NotNullable();
            Alter.Column("UserId").OnTable("DelegateAccounts").AsInt32().NotNullable();

            Delete.Index("IX_DelegateAccounts_CentreID_EmailAddress").OnTable("DelegateAccounts");
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

            Create.UniqueConstraint("IX_DelegateAccounts_UserId_CentreId").OnTable("DelegateAccounts")
                .Columns("UserId", "CentreId");
            Create.UniqueConstraint("IX_AdminAccounts_UserId_CentreId").OnTable("AdminAccounts")
                .Columns("UserId", "CentreId");
        }

        public override void Down()
        {
            Delete.UniqueConstraint("IX_AdminAccounts_UserId_CentreId").FromTable("AdminAccounts");
            Delete.UniqueConstraint("IX_DelegateAccounts_UserId_CentreId").FromTable("DelegateAccounts");

            Delete.Index("IX_AdminAccounts_CentreId_Email").OnTable("AdminAccounts");

            Delete.Index("IX_DelegateAccounts_CentreId_Email").OnTable("DelegateAccounts");
            Create.Index("IX_DelegateAccounts_CentreID_EmailAddress").OnTable("DelegateAccounts").OnColumn("CentreID")
                .Ascending()
                .OnColumn("Email").Ascending().WithOptions().NonClustered();

            Alter.Column("UserId").OnTable("AdminAccounts").AsInt32().Nullable();
            Alter.Column("UserId").OnTable("DelegateAccounts").AsInt32().Nullable();
        }
    }
}
