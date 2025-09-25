namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202504281115)]
    public class UpdateFrameworkCompetenciesOrdering : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@"WITH Ranked AS (
                        SELECT ID, 
                               ROW_NUMBER() OVER (PARTITION BY FrameworkID ORDER BY SysStartTime) AS NewOrder
                        FROM FrameworkCompetencies
	                    Where FrameworkCompetencyGroupID  is null
                        )
                        UPDATE fc
                        SET fc.Ordering = r.NewOrder
                        FROM FrameworkCompetencies fc
                        JOIN Ranked r ON fc.ID = r.ID;");
        }
    }
}
