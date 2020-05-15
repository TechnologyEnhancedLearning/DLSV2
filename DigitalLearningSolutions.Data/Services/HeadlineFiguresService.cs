namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface IHeadlineFiguresService
    {
        HeadlineFigures GetHeadlineFigures();
    }

    public class HeadlineFiguresService : IHeadlineFiguresService
    {
        private readonly IDbConnection connection;

        public HeadlineFiguresService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public HeadlineFigures GetHeadlineFigures()
        {
            return connection.QueryFirst<HeadlineFigures>(@"
                SELECT
                (
                    SELECT COUNT(CandidateID)
                    FROM Candidates
                )
                AS Delegates,
                (
                    SELECT COUNT(ProgressID)
                    FROM Progress
                    WHERE Completed IS NOT NULL
                )
                AS Completions,
                (
                    SELECT SUM(Duration)
                    FROM Sessions
                ) / 60
                AS LearningTime,
                (
                    SELECT COUNT(CentreName)
                    FROM Centres AS ce
                    WHERE Active = 1
                    AND (
                        DATEADD(M, 6, (
                            SELECT MAX(se.LoginTime)
                            FROM Sessions AS se
                            INNER JOIN Customisations
                            AS cu ON se.CustomisationID = cu.CustomisationID
                            WHERE cu.CentreID = ce.CentreID
                        )) > GETDATE()
                    )
                )
                AS ActiveCentres
            ");
        }
    }
}
