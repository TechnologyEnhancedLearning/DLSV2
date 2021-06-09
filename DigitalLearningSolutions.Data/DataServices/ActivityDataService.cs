namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface IActivityDataService
    {
        int GetActivityForMonthAndYear(int year, int month, string activityType);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int GetActivityForMonthAndYear(int year, int month, string activityType)
        {
            return connection.Query<int>(
                @"SELECT COUNT (LogID)
                        FROM tActivityLog
                        WHERE @activityType = 1
                        AND LogYear = @year
                        AND LogMonth = @month",
            new {year, month, activityType}
            ).First();
        }
    }
}
