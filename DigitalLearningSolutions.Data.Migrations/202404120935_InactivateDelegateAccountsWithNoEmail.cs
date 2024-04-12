namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202404120935)]
    public class InactivateDelegateAccountsWithNoEmail : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(
                @$"UPDATE DelegateAccounts
                    SET       Active = 1
                    FROM   Users AS u INNER JOIN
                                 DelegateAccounts ON u.ID = DelegateAccounts.UserID
                    WHERE (NOT (u.PrimaryEmail LIKE N'%@%')) AND (DelegateAccounts.Active = 1)"
                );
        }
    }
}
