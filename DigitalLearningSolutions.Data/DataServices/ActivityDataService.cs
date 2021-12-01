namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IActivityDataService
    {
        IEnumerable<ActivityLog> GetFilteredActivity(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        );

        DateTime GetStartOfActivityForCentre(int centreId);
    }

    public class ActivityDataService : IActivityDataService
    {
        private readonly IDbConnection connection;

        public ActivityDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<ActivityLog> GetFilteredActivity(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        )
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
                    FROM tActivityLog AS al
                    JOIN Applications AS ap ON ap.CustomisationID = al.CustomisationID
                        WHERE (LogDate >= @startDate
                            AND (@endDate IS NULL OR LogDate <= @endDate)
                            AND CentreID = @centreId
                            AND (@jobGroupId IS NULL OR JobGroupID = @jobGroupId)
                            AND (@customisationId IS NULL OR CustomisationID = @customisationId)
                            AND (@courseCategoryId IS NULL OR CourseCategoryId = @courseCategoryId)
                            AND (Registered = 1 OR Completed = 1 OR Evaluated = 1))
                            AND ap.DefaultContentTypeID <> 4",
                new
                {
                    centreId,
                    startDate,
                    endDate,
                    jobGroupId,
                    customisationId,
                    courseCategoryId
                }
            );
        }

        public DateTime GetStartOfActivityForCentre(int centreId)
        {
            return connection.QuerySingleOrDefault<DateTime>(
                @"SELECT MIN(LogDate)
                    FROM tActivityLog
                    WHERE CentreID = @centreId",
                new { centreId }
            );
        }
    }
}
