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
            DateTime endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        );

        EvaluationSummaryData GetEvaluationSummaryData(
            int centreId,
            DateTime startDate,
            DateTime endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        );
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
            DateTime endDate,
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
                    FROM tActivityLog
                        WHERE (LogDate >= @startDate
                            AND LogDate <= @endDate
                            AND CentreID = @centreId
                            AND (@jobGroupId IS NULL OR JobGroupID = @jobGroupId)
                            AND (@customisationId IS NULL OR CustomisationID = @customisationId)
                            AND (@courseCategoryId IS NULL OR CourseCategoryId = @courseCategoryId)
                            AND (Registered = 1 OR Completed = 1 OR Evaluated = 1))",
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

        public EvaluationSummaryData GetEvaluationSummaryData(
            int centreId,
            DateTime startDate,
            DateTime endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        )
        {
            return connection.QuerySingle<EvaluationSummaryData>(
                @"SELECT
					    SUM(CASE WHEN Q1 = 0 THEN 1 ELSE 0 END) AS Q1No,
					    SUM(CASE WHEN Q1 = 1 THEN 1 ELSE 0 END) AS Q1Yes,
					    SUM(CASE WHEN Q1 = 255 THEN 1 ELSE 0 END) AS Q1NoResponse,
					    
					    SUM(CASE WHEN Q2 = 0 THEN 1 ELSE 0 END) AS Q2No,
					    SUM(CASE WHEN Q2 = 1 THEN 1 ELSE 0 END) AS Q2Yes,
					    SUM(CASE WHEN Q2 = 255 THEN 1 ELSE 0 END) AS Q2NoResponse,

					    SUM(CASE WHEN Q3 = 0 THEN 1 ELSE 0 END) AS Q3No,
					    SUM(CASE WHEN Q3 = 1 THEN 1 ELSE 0 END) AS Q3Yes,
					    SUM(CASE WHEN Q3 = 255 THEN 1 ELSE 0 END) AS Q3NoResponse,
					    
					    SUM(CASE WHEN Q3 = 0 THEN 1 ELSE 0 END) AS Q4Hrs0,
					    SUM(CASE WHEN (Q3 != 0 and Q4 = 1) THEN 1 ELSE 0 END) AS Q4HrsLt1,
					    SUM(CASE WHEN (Q3 != 0 and Q4 = 2) THEN 1 ELSE 0 END) AS Q4Hrs1To2,
					    SUM(CASE WHEN (Q3 != 0 and Q4 = 3) THEN 1 ELSE 0 END) AS Q4Hrs2To4,
					    SUM(CASE WHEN (Q3 != 0 and Q4 = 4) THEN 1 ELSE 0 END) AS Q4Hrs4To6,
					    SUM(CASE WHEN (Q3 != 0 and Q4 = 5) THEN 1 ELSE 0 END) AS Q4HrsGt6,
					    SUM(CASE WHEN (Q3 != 0 and Q4 = 255) THEN 1 ELSE 0 END) AS Q4NoResponse,
					    
					    SUM(CASE WHEN Q5 = 0 THEN 1 ELSE 0 END) AS Q5No,
					    SUM(CASE WHEN Q5 = 1 THEN 1 ELSE 0 END) AS Q5Yes,
					    SUM(CASE WHEN Q5 = 255 THEN 1 ELSE 0 END) AS Q5NoResponse,
					    
					    SUM(CASE WHEN Q6 = 0 THEN 1 ELSE 0 END) AS Q6NotApplicable,
					    SUM(CASE WHEN Q6 = 1 THEN 1 ELSE 0 END) AS Q6No,
					    SUM(CASE WHEN Q6 = 3 THEN 1 ELSE 0 END) AS Q6YesIndirectly,
					    SUM(CASE WHEN Q6 = 2 THEN 1 ELSE 0 END) AS Q6YesDirectly,
					    SUM(CASE WHEN Q6 = 255 THEN 1 ELSE 0 END) AS Q6NoResponse,
					    
					    SUM(CASE WHEN Q7 = 0 THEN 1 ELSE 0 END) AS Q7No,
					    SUM(CASE WHEN Q7 = 1 THEN 1 ELSE 0 END) AS Q7Yes,
					    SUM(CASE WHEN Q7 = 255 THEN 1 ELSE 0 END) AS Q7NoResponse
                    FROM Evaluations e
                    INNER JOIN Customisations c ON e.CustomisationID = c.CustomisationID
                    WHERE EvaluatedDate >= @startDate
                        AND EvaluatedDate <= @endDate
                        AND (@customisationId IS NULL OR c.CustomisationID = @customisationId)
                        AND CentreID = @centreId",
                new { startDate, endDate, customisationId, centreId }
            );
        }
    }
}
