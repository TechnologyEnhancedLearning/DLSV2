using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202401161703)]
    public class UpdateCandidateAssessments_SetDateMinValueToNull : Migration
    {
        public override void Up()
        {
            Execute.Sql(
                @$"UPDATE CandidateAssessments SET CompleteByDate = NULL
                    WHERE CompleteByDate = '1900-01-01 00:00:00.000';"
            );
        }

        public override void Down()
        {
        }
    }
}

