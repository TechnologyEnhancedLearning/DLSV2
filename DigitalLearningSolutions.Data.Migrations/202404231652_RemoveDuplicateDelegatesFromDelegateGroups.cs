namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202404231652)]
    public class RemoveDuplicateDelegatesFromDelegateGroups : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(
                @$"WITH GroupDelegatesCTE AS (
                        SELECT *,
                               ROW_NUMBER() OVER (PARTITION BY GroupID, DelegateID ORDER BY GroupDelegateID) AS rn
                        FROM GroupDelegates
                    )
                    DELETE FROM GroupDelegatesCTE WHERE rn > 1;"
                );
        }
    }
}
