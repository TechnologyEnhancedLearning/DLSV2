namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityDataService
    {
        IEnumerable<MonthOfActivity> GetActivityForMonthsInYear(int year, IEnumerable<int> months);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<MonthOfActivity> GetActivityForMonthsInYear(int year, IEnumerable<int> months)
        {
            return connection.Query<IEnumerable<MonthOfActivity>>(
                @"SELECT
                        LogYear AS Year,
                        LogMonth AS Month,
                        SUM(CONVERT(INT, Completed)) AS Completions,
                        SUM(CONVERT(INT, Evaluated)) AS Evaluations,
                        SUM(CONVERT(INT, Registered)) AS Registrations 
                    FROM tActivityLog
                        WHERE (LogYear = @year AND LogMonth IN @months)
                    GROUP BY LogYear, LogMonth
                    ORDER BY LogYear, LogMonth",
            new {year, months}
            ).First();
        }
    }
}
