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

        DateTime? GetStartOfActivityForCentre(int centreId, int? courseCategoryId = null);
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
                        Cast(LogDate As Date) As LogDate,
                        LogYear,
                        LogQuarter,
                        LogMonth,
                        SUM(CAST(Registered AS Int)) AS Registered,
						SUM(CAST(Completed AS Int)) AS Completed,
						SUM(CAST(Evaluated AS Int)) AS Evaluated
                    FROM tActivityLog AS al
                    WHERE (LogDate >= @startDate
                        AND (@endDate IS NULL OR LogDate <= @endDate)
                        AND CentreID = @centreId
                        AND (@jobGroupId IS NULL OR JobGroupID = @jobGroupId)
                        AND (@customisationId IS NULL OR al.CustomisationID = @customisationId)
                        AND (@courseCategoryId IS NULL OR al.CourseCategoryId = @courseCategoryId)
                        AND (Registered = 1 OR Completed = 1 OR Evaluated = 1))
                        AND EXISTS (
                            SELECT ap.ApplicationID
                            FROM Applications ap
                            WHERE ap.ApplicationID = al.ApplicationID
                            AND ap.DefaultContentTypeID <> 4)
                    GROUP BY  Cast(LogDate As Date), LogYear,
                        LogQuarter,
                        LogMonth",
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

        public DateTime? GetStartOfActivityForCentre(int centreId, int? courseCategoryId = null)
        {
            return connection.QuerySingleOrDefault<DateTime?>(
                @"SELECT MIN(LogDate)
                    FROM tActivityLog
                    WHERE CentreID = @centreId AND (CourseCategoryID = @courseCategoryId OR @courseCategoryId IS NULL)",
                new { centreId, courseCategoryId }
            );
        }
    }
}
