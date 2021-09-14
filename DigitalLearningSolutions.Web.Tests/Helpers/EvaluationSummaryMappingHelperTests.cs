namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryMappingHelperTests
    {
        [Test]
        public void GenerateNumberWithLimitDisplayString_returns_expected_string_with_limit()
        {
            // Given
            var data = EvaluationSummaryTestHelper.GetDefaultEvaluationSummaryData();
            var expectedResults = new List<EvaluationSummaryViewModel>
            {
                new EvaluationSummaryViewModel(
                    "Increased productivity?",
                    new Dictionary<string, int>
                        { { "Yes", 107 }, { "No", 102 }, { "No response", 7 } },
                    7
                ),
                new EvaluationSummaryViewModel(
                    "Gained new skills?",
                    new Dictionary<string, int>
                        { { "Yes", 102 }, { "No", 107 }, { "No response", 7 } },
                    7
                ),
                new EvaluationSummaryViewModel(
                    "Perform faster?",
                    new Dictionary<string, int>
                        { { "Yes", 112 }, { "No", 95 }, { "No response", 9 } },
                    9
                ),
                new EvaluationSummaryViewModel(
                    "Time saving per week",
                    new Dictionary<string, int>
                    {
                        { "0 hrs", 95 }, { "<1 hr", 54 }, { "1-2 hrs", 30 },
                        { "2-4 hrs", 16 }, { "4-6 hrs", 7 }, { ">6 hrs", 3 },
                        { "No response", 11 }
                    },
                    11
                ),
                new EvaluationSummaryViewModel(
                    "Pass on skills?",
                    new Dictionary<string, int>
                        { { "Yes", 136 }, { "No", 64 }, { "No response", 16 } },
                    16
                ),
                new EvaluationSummaryViewModel(
                    "Help with patients/clients?",
                    new Dictionary<string, int>
                    {
                        { "Yes, directly", 34 }, { "Yes, indirectly", 86 },
                        { "No", 64 }, { "No response", 9 }
                    },
                    64
                ),
                new EvaluationSummaryViewModel(
                    "Recommended materials?",
                    new Dictionary<string, int>
                        { { "Yes", 157 }, { "No", 51 }, { "No response", 8 } },
                    8
                )
            };

            // When
            var result = EvaluationSummaryMappingHelper.MapDataToEvaluationSummaryViewModels(data);

            // Then
            result.Should().BeEquivalentTo(expectedResults);
        }
    }
}
