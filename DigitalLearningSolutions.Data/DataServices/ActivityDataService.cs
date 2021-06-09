namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityDataService
    {
        MonthOfActivity GetActivityForMonthAndYear(int year, int month);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public MonthOfActivity GetActivityForMonthAndYear(int year, int month)
        {
            return connection.Query<MonthOfActivity>(
                @"SELECT
                        @year AS Year,
                        @month AS Month,
                        SUM(CONVERT(INT, Completed)) AS Completions,
                        SUM(CONVERT(INT, Evaluated)) AS Evaluations,
                        SUM(CONVERT(INT, Registered)) AS Registrations 
                    FROM tActivityLog
                        WHERE LogYear = @year
                        AND LogMonth = @month",
            new {year, month}
            ).First();
        }
    }
}
