namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;

    public static class EvaluationSummaryTestHelper
    {
        public static EvaluationAnswerCounts GetDefaultEvaluationAnswerCounts()
        {
            return new EvaluationAnswerCounts
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

        public static IEnumerable<EvaluationResponseBreakdown> GetDefaultEvaluationResponseBreakdowns()
        {
            return new List<EvaluationResponseBreakdown>
            {
                new EvaluationResponseBreakdown(
                    "Increased productivity?",
                    new List<(string question, int count)>
                    {
                        ("Yes", 107),
                        ("No", 102),
                        ("No response", 7)
                    }
                ),
                new EvaluationResponseBreakdown(
                    "Gained new skills?",
                    new List<(string question, int count)>
                    {
                        ("Yes", 102),
                        ("No", 107),
                        ("No response", 7)
                    }
                ),
                new EvaluationResponseBreakdown(
                    "Perform faster?",
                    new List<(string question, int count)>
                    {
                        ("Yes", 112),
                        ("No", 95),
                        ("No response", 9)
                    }
                ),
                new EvaluationResponseBreakdown(
                    "Time saving per week",
                    new List<(string question, int count)>
                    {
                        ("0 hrs", 95),
                        ("Less than 1 hr", 54),
                        ("1 to 2 hrs", 30),
                        ("2 to 4 hrs", 16),
                        ("4 to 6 hrs", 7),
                        ("More than 6 hrs", 3),
                        ("No response", 11)
                    }
                ),
                new EvaluationResponseBreakdown(
                    "Pass on skills?",
                    new List<(string question, int count)>
                    {
                        ("Yes", 136),
                        ("No", 64),
                        ("No response", 16)
                    }
                ),
                new EvaluationResponseBreakdown(
                    "Help with patients/clients?",
                    new List<(string question, int count)>
                    {
                        ("Yes, directly", 34),
                        ("Yes, indirectly", 86),
                        ("No", 64),
                        ("No response", 9)
                    }
                ),
                new EvaluationResponseBreakdown(
                    "Recommended materials?",
                    new List<(string question, int count)>
                    {
                        ("Yes", 157),
                        ("No", 51),
                        ("No response", 8)
                    }
                )
            };
        }
    }
}
