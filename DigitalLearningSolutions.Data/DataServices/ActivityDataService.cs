namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityDataService
    {
        IEnumerable<MonthOfActivity> GetActivityInRangeByMonth(int centreId, DateTime start, DateTime end);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<MonthOfActivity> GetActivityInRangeByMonth(int centreId, DateTime start, DateTime end)
        {
            return connection.Query<MonthOfActivity>(
                @"SELECT
                        LogYear AS Year,
                        LogMonth AS Month,
                        SUM(CONVERT(INT, Completed)) AS Completions,
                        SUM(CONVERT(INT, Evaluated)) AS Evaluations,
                        SUM(CONVERT(INT, Registered)) AS Registrations 
                    FROM tActivityLog
                        WHERE (LogDate > @start AND LogDate < @end AND CentreID = @centreId)
                    GROUP BY LogYear, LogMonth
                    ORDER BY LogYear, LogMonth",
                new { centreId, start, end }
            );
        }
    }
}
