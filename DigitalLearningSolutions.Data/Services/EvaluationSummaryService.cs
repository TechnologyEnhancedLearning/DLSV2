namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public interface IEvaluationSummaryService
    {
        IEnumerable<EvaluationResponseBreakdown> GetEvaluationSummary(int centreId, ActivityFilterData filterData);
    }

    public class EvaluationSummaryService : IEvaluationSummaryService
    {
        private readonly IEvaluationSummaryDataService evaluationSummaryDataService;

        public EvaluationSummaryService(IEvaluationSummaryDataService evaluationSummaryDataService)
        {
            this.evaluationSummaryDataService = evaluationSummaryDataService;
        }

        public IEnumerable<EvaluationResponseBreakdown> GetEvaluationSummary(
            int centreId,
            ActivityFilterData filterData
        )
        {
            var evaluationSummaryData = evaluationSummaryDataService.GetEvaluationSummaryData(
                centreId,
                filterData.StartDate,
                filterData.EndDate,
                filterData.JobGroupId,
                filterData.CourseCategoryId,
                filterData.CustomisationId
            );
            return MapDataToEvaluationResponseBreakdowns(evaluationSummaryData);
        }

        private static IEnumerable<EvaluationResponseBreakdown> MapDataToEvaluationResponseBreakdowns(
            EvaluationAnswerCounts data
        )
        {
            var q1 = new EvaluationResponseBreakdown(
                "Increased productivity?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q1Yes),
                    ("No", data.Q1No),
                    ("No response", data.Q1NoResponse)
                }
            );
            var q2 = new EvaluationResponseBreakdown(
                "Gained new skills?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q2Yes),
                    ("No", data.Q2No),
                    ("No response", data.Q2NoResponse)
                }
            );
            var q3 = new EvaluationResponseBreakdown(
                "Perform faster?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q3Yes),
                    ("No", data.Q3No),
                    ("No response", data.Q3NoResponse)
                }
            );
            var q4 = new EvaluationResponseBreakdown(
                "Time saving per week",
                new List<(string response, int count)>
                {
                    ("0 hrs", data.Q4Hrs0),
                    ("<1 hr", data.Q4HrsLt1),
                    ("1-2 hrs", data.Q4Hrs1To2),
                    ("2-4 hrs", data.Q4Hrs2To4),
                    ("4-6 hrs", data.Q4Hrs4To6),
                    (">6 hrs", data.Q4HrsGt6),
                    ("No response", data.Q4NoResponse)
                }
            );
            var q5 = new EvaluationResponseBreakdown(
                "Pass on skills?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q5Yes),
                    ("No", data.Q5No),
                    ("No response", data.Q5NoResponse)
                }
            );
            var q6 = new EvaluationResponseBreakdown(
                "Help with patients/clients?",
                new List<(string response, int count)>
                {
                    ("Yes, directly", data.Q6YesDirectly),
                    ("Yes, indirectly", data.Q6YesIndirectly),
                    ("No", data.Q6No),
                    ("No response", data.Q6NoResponse)
                }
            );
            var q7 = new EvaluationResponseBreakdown(
                "Recommended materials?",
                new List<(string response, int count)>
                {
                    ("Yes", data.Q7Yes),
                    ("No", data.Q7No),
                    ("No response", data.Q7NoResponse)
                }
            );
            return new List<EvaluationResponseBreakdown> { q1, q2, q3, q4, q5, q6, q7 };
        }
    }
}
