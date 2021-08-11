namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityDataService
    {
        IEnumerable<ActivityLog> GetRawActivity(int centreId, ActivityFilterData filterData);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<ActivityLog> GetRawActivity(int centreId, ActivityFilterData filterData)
        {
            return connection.Query<ActivityLog>(
                @"SELECT
                        LogDate,
                        LogYear,
                        LogQuarter,
                        LogMonth,
                        Registered,
                        Completed,
                        Evaluated
                    FROM tActivityLog
                        WHERE (LogDate > @startDate
                            AND LogDate < @endDate
                            AND CentreID = @centreId
                            AND (@jobGroupId = 0 OR JobGroupID = @jobGroupId)
                            AND (@customisationId = 0 OR CustomisationID = @customisationId)
                            AND (@courseCategoryId = 0 OR CourseCategoryId = @courseCategoryId)
                            AND (Registered = 1 OR Completed = 1 OR Evaluated = 1))
                        ORDER BY LogDate",
                new
                {
                    centreId,
                    filterData.StartDate,
                    filterData.EndDate,
                    filterData.JobGroupId,
                    filterData.CustomisationId,
                    filterData.CourseCategoryId
                }
            );
        }
    }
}
