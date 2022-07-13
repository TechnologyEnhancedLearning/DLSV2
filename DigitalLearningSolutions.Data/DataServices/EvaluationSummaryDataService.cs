namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IEvaluationSummaryDataService
    {
        EvaluationAnswerCounts GetEvaluationSummaryData(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        );
    }

    public class EvaluationSummaryDataService : IEvaluationSummaryDataService
    {
        private readonly IDbConnection connection;

        public EvaluationSummaryDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public EvaluationAnswerCounts GetEvaluationSummaryData(
            int centreId,
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        )
        {
            return connection.QuerySingle<EvaluationAnswerCounts>(
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
                        SUM(CASE WHEN (Q3 != 0 AND Q4 = 1) THEN 1 ELSE 0 END) AS Q4HrsLt1,
                        SUM(CASE WHEN (Q3 != 0 AND Q4 = 2) THEN 1 ELSE 0 END) AS Q4Hrs1To2,
                        SUM(CASE WHEN (Q3 != 0 AND Q4 = 3) THEN 1 ELSE 0 END) AS Q4Hrs2To4,
                        SUM(CASE WHEN (Q3 != 0 AND Q4 = 4) THEN 1 ELSE 0 END) AS Q4Hrs4To6,
                        SUM(CASE WHEN (Q3 != 0 AND Q4 = 5) THEN 1 ELSE 0 END) AS Q4HrsGt6,
                        SUM(CASE WHEN (Q3 != 0 AND Q4 = 255) THEN 1 ELSE 0 END) AS Q4NoResponse,
                        
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
                    INNER JOIN Customisations c ON c.CustomisationID = e.CustomisationID
                    INNER JOIN Applications a ON a.ApplicationID = c.ApplicationID
                    WHERE c.CentreID = @centreId
                        AND e.EvaluatedDate >= @startDate
                        AND (@endDate IS NULL OR e.EvaluatedDate <= @endDate)
                        AND (@jobGroupId IS NULL OR e.JobGroupID = @jobGroupId)
                        AND (@customisationId IS NULL OR e.CustomisationID = @customisationId)
                        AND (@courseCategoryId IS NULL OR a.CourseCategoryId = @courseCategoryId)
                        AND a.DefaultContentTypeID <> 4",
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
    }
}
