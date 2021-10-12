namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202110121601)]
    public class UpdateSelfAssessmentStructureOrdering : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"WITH T AS (SELECT ID, SelfAssessmentID, CompetencyID, Ordering, ROW_NUMBER() OVER(Partition By SelfAssessmentID Order By ID) As NewOrdering
                            FROM            SelfAssessmentStructure
                            WHERE Ordering = 1)
                            UPDATE SelfAssessmentStructure
                            SET Ordering = T.NewOrdering
                            FROM SelfAssessmentStructure AS sas INNER JOIN T ON T.ID = sas.ID
                            WHERE T.NewOrdering > 1");
        }
        public override void Down()
        {

        }
    }
}
