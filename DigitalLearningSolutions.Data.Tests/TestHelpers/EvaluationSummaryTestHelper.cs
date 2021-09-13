namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public static class EvaluationSummaryTestHelper
    {
        public static EvaluationSummaryData GetDefaultEvaluationSummaryData()
        {
            return new EvaluationSummaryData
            {
                Q1No = 102,
                Q1Yes = 107,
                Q1NoResponse = 7,
                Q2No = 107,
                Q2Yes = 102,
                Q2NoResponse = 7,
                Q3No = 95,
                Q3Yes = 112,
                Q3NoResponse = 9,
                Q4Hrs0 = 95,
                Q4HrsLt1 = 54,
                Q4Hrs1To2 = 30,
                Q4Hrs2To4 = 16,
                Q4Hrs4To6 = 7,
                Q4HrsGt6 = 3,
                Q4NoResponse = 11,
                Q5No = 64,
                Q5Yes = 136,
                Q5NoResponse = 16,
                Q6NotApplicable = 23,
                Q6No = 64,
                Q6YesIndirectly = 86,
                Q6YesDirectly = 34,
                Q6NoResponse = 9,
                Q7No = 51,
                Q7Yes = 157,
                Q7NoResponse = 8
            };
        }
    }
}
