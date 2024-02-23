namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202401301650)]
    public class AddGroupDelegatesConstraints : Migration
    {
        public override void Up()
        {
            Execute.Sql(
                @"WITH duplicateRowNum AS (
                    SELECT *, ROW_NUMBER() OVER (PARTITION BY GroupId,DelegateId ORDER BY AddedDate) rownum
                    FROM GroupDelegates
                    )
                DELETE FROM duplicateRowNum WHERE rownum > 1;"
            );

            Create.UniqueConstraint("UQ_GroupDelegates_GroupID_DelegateID").OnTable("GroupDelegates")
                .Columns("GroupID","DelegateID");
        }

        public override void Down()
        {
            Delete.UniqueConstraint("UQ_GroupDelegates_GroupID_DelegateID").FromTable("GroupDelegates");
        }
    }
}
