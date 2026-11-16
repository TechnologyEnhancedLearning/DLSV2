namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202608051210)]
    public class AddIndexGetAndReturnCompletionStatusByProgID : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                            CREATE NONCLUSTERED INDEX IX_AssessAttempts_ProgressID_SectionNumber_Passed
                                ON dbo.AssessAttempts (ProgressID, SectionNumber)
                                WHERE Status = 1;
                        ");
        }

        public override void Down()
        {
            Execute.Sql(@"
                            DROP INDEX IX_AssessAttempts_ProgressID_SectionNumber_Passed
                                ON dbo.AssessAttempts;
                        ");
        }
    }
}
