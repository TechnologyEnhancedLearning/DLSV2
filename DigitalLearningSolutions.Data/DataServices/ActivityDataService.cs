namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityDataService
    {
        IEnumerable<MonthOfActivity> GetActivityForMonthsInYear(int centreId, int year, IEnumerable<int> months);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<MonthOfActivity> GetActivityForMonthsInYear(int centreId, int year, IEnumerable<int> months)
        {
            return connection.Query<MonthOfActivity>(
                @"DECLARE @monthTable TABLE (Month INT)
                    INSERT @monthTable VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12)
                    
                    DECLARE @activity TABLE (Year INT, Month INT, Completions INT, Evaluations INT, Registrations INT)
                    INSERT @activity SELECT
                        LogYear AS Year,
                        LogMonth AS Month,
                        SUM(CONVERT(INT, Completed)) AS Completions,
                        SUM(CONVERT(INT, Evaluated)) AS Evaluations,
                        SUM(CONVERT(INT, Registered)) AS Registrations 
                    FROM tActivityLog
                        WHERE (LogYear = @year AND LogMonth IN @months AND CentreID = @centreId)
                    GROUP BY LogYear, LogMonth
                    
                    SELECT
                        @year AS Year,
                        m.Month,
                        COALESCE(a.Completions, 0) AS Completions,
                        COALESCE(a.Evaluations, 0) AS Evaluations,
                        COALESCE(a.Registrations, 0) AS Registrations
                    FROM @monthTable m
	                    LEFT JOIN @activity a ON m.Month = a.Month
                    WHERE m.Month IN @months
                    ORDER BY m.Month DESC",
            new {centreId, year, months}
            );
        }
    }
}
