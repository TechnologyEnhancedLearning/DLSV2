namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using System.Diagnostics.Metrics;

    [Migration(202408271610)]
    public class AlterConstraintForProgressAddUtcDate : Migration
    {
        public override void Up()
        {
            Execute.Sql(@$"ALTER TABLE [dbo].[Progress] DROP CONSTRAINT [DF_Progress_FirstSubmittedTime];
                           ALTER TABLE [dbo].[Progress] ADD CONSTRAINT [DF_Progress_FirstSubmittedTime] DEFAULT (GETUTCDATE()) FOR [FirstSubmittedTime];");
        }
        public override void Down()
        {
            Execute.Sql(@$"ALTER TABLE [dbo].[Progress] DROP CONSTRAINT [DF_Progress_FirstSubmittedTime];
                           ALTER TABLE [dbo].[Progress] ADD CONSTRAINT [DF_Progress_FirstSubmittedTime] DEFAULT (GETDATE()) FOR [FirstSubmittedTime];");
        }

    }
}
