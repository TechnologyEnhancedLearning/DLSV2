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
        int GetActivityDetailRowCount(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
            );
        IEnumerable<ActivityLogDetail> GetFilteredActivityDetail(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId,
            int exportQueryRowLimit,
            int currentRun
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
        public int GetActivityDetailRowCount(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
            )
        {
            return connection.QuerySingleOrDefault<int>(
                @"SELECT COUNT(1) FROM
                                 tActivityLog AS al
                        WHERE(al.LogDate >= @startDate) AND(@endDate IS NULL OR
                                 al.LogDate <= @endDate) AND(al.CentreID = @centreId) AND(@jobGroupId IS NULL OR
                                 al.JobGroupID = @jobGroupId) AND(@customisationId IS NULL OR
                                 al.CustomisationID = @customisationId) AND(@courseCategoryId IS NULL OR
                                 al.CourseCategoryID = @courseCategoryId) AND(al.Registered = 1 OR
                                 al.Completed = 1 OR
                                 al.Evaluated = 1) AND EXISTS
                                 (SELECT ApplicationID
                                     FROM    Applications AS ap
                                     WHERE (ApplicationID = al.ApplicationID) AND
                                     (DefaultContentTypeID<> 4))",
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
        public IEnumerable<ActivityLogDetail> GetFilteredActivityDetail(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId,
            int exportQueryRowLimit,
            int currentRun
            )
        {
            return connection.Query<ActivityLogDetail>(
                @"SELECT al.LogID,
                    al.LogDate,
                    a.ApplicationName AS CourseName,
                    c.CustomisationName,
                    u.FirstName,
                    u.LastName,
                    u.PrimaryEmail,
                    da.CandidateNumber AS DelegateId,
                    da.Answer1,
                    da.Answer2,
                    da.Answer3,
                    da.Answer4,
                    da.Answer5,
                    da.Answer6,
                    al.Registered,
                    al.Completed,
                    al.Evaluated
                    FROM   Applications AS a INNER JOIN
                                 tActivityLog AS al ON a.ApplicationID = al.ApplicationID INNER JOIN
                                 Users AS u INNER JOIN
                                 DelegateAccounts AS da ON u.ID = da.UserID ON al.CandidateID = da.ID INNER JOIN
                                 Customisations AS c ON al.CustomisationID = c.CustomisationID
                    WHERE (al.LogDate >= @startDate) AND (@endDate IS NULL OR
                                 al.LogDate <= @endDate) AND (al.CentreID = @centreId) AND (@jobGroupId IS NULL OR
                                 al.JobGroupID = @jobGroupId) AND (@customisationId IS NULL OR
                                 al.CustomisationID = @customisationId) AND (@courseCategoryId IS NULL OR
                                 al.CourseCategoryID = @courseCategoryId) AND (al.Registered = 1 OR
                                 al.Completed = 1 OR
                                 al.Evaluated = 1) AND EXISTS
                                     (SELECT ApplicationID
                                    FROM    Applications AS ap
                                    WHERE (ApplicationID = al.ApplicationID) AND (DefaultContentTypeID <> 4))
                    ORDER BY al.LogDate DESC
                            OFFSET @exportQueryRowLimit * (@currentRun - 1) ROWS
                            FETCH NEXT @exportQueryRowLimit ROWS ONLY"
                ,
                new
                {
                    centreId,
                    startDate,
                    endDate,
                    jobGroupId,
                    customisationId,
                    courseCategoryId,
                    exportQueryRowLimit,
                    currentRun
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
