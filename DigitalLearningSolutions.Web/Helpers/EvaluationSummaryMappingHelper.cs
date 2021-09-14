namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;

    public static class EvaluationSummaryMappingHelper
    {
        public static IEnumerable<EvaluationSummaryViewModel> MapDataToEvaluationSummaryViewModels(
            EvaluationSummaryData data
        )
        {
            var q1 = new EvaluationSummaryViewModel(
                "Increased productivity?",
                new Dictionary<string, int>
                    { { "Yes", data.Q1Yes }, { "No", data.Q1No }, { "No response", data.Q1NoResponse } },
                data.Q1NoResponse
            );
            var q2 = new EvaluationSummaryViewModel(
                "Gained new skills?",
                new Dictionary<string, int>
                    { { "Yes", data.Q2Yes }, { "No", data.Q2No }, { "No response", data.Q2NoResponse } },
                data.Q2NoResponse
            );
            var q3 = new EvaluationSummaryViewModel(
                "Perform faster?",
                new Dictionary<string, int>
                    { { "Yes", data.Q3Yes }, { "No", data.Q3No }, { "No response", data.Q3NoResponse } },
                data.Q3NoResponse
            );
            var q4 = new EvaluationSummaryViewModel(
                "Time saving per week",
                new Dictionary<string, int>
                {
                    { "0 hrs", data.Q4Hrs0 }, { "<1 hr", data.Q4HrsLt1 }, { "1-2 hrs", data.Q4Hrs1To2 },
                    { "2-4 hrs", data.Q4Hrs2To4 }, { "4-6 hrs", data.Q4Hrs4To6 }, { ">6 hrs", data.Q4HrsGt6 },
                    { "No response", data.Q4NoResponse }
                },
                data.Q4NoResponse
            );
            var q5 = new EvaluationSummaryViewModel(
                "Pass on skills?",
                new Dictionary<string, int>
                    { { "Yes", data.Q5Yes }, { "No", data.Q5No }, { "No response", data.Q5NoResponse } },
                data.Q5NoResponse
            );
            var q6 = new EvaluationSummaryViewModel(
                "Help with patients/clients?",
                new Dictionary<string, int>
                {
                    { "Yes, directly", data.Q6YesDirectly }, { "Yes, indirectly", data.Q6YesIndirectly },
                    { "No", data.Q6No }, { "No response", data.Q6NoResponse }
                },
                data.Q6NoResponse
            );
            var q7 = new EvaluationSummaryViewModel(
                "Recommended materials?",
                new Dictionary<string, int>
                    { { "Yes", data.Q7Yes }, { "No", data.Q7No }, { "No response", data.Q7NoResponse } },
                data.Q7NoResponse
            );
            return new List<EvaluationSummaryViewModel>
                { q1, q2, q3, q4, q5, q6, q7 };
        }
    }
}
