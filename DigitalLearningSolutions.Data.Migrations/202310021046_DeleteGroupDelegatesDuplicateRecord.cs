using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202310021046)]
    public class GroupDelegatesDuplicateRecordDelete : Migration
    {
        public override void Up()
        {
            Execute.Sql(@$"WITH CTE AS
                            (
                                SELECT *,ROW_NUMBER() OVER (PARTITION BY GroupID,DelegateID,convert(date,addedDate) ORDER BY GroupID,DelegateID,convert(date,addedDate)) AS RN
                                FROM  GroupDelegates
                            )

                            DELETE FROM CTE WHERE RN>1"
                        );
        }
        public override void Down()
        {

        }
    }
}
