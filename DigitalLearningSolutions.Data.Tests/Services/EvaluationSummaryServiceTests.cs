namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryServiceTests
    {
        private IEvaluationSummaryDataService evaluationSummaryDataService = null!;
        private IEvaluationSummaryService evaluationSummaryService = null!;

        [SetUp]
        public void SetUp()
        {
            evaluationSummaryDataService = A.Fake<IEvaluationSummaryDataService>();
            evaluationSummaryService = new EvaluationSummaryService(evaluationSummaryDataService);
        }

        [Test]
        public void GetEvaluationSummaryModels_returns_list_of_models_correctly()
        {
            // Given
            var data = EvaluationSummaryTestHelper.GetDefaultEvaluationSummaryData();
            const int centreId = 121;
            var activityFilterData = new ActivityFilterData(
                DateTime.Today,
                DateTime.Today,
                null,
                null,
                null,
                ReportInterval.Months
            );
            A.CallTo(
                () => evaluationSummaryDataService.GetEvaluationSummaryData(
                    centreId,
                    activityFilterData.StartDate,
                    activityFilterData.EndDate,
                    null,
                    null,
                    null
                )
            ).Returns(data);
            var expectedResults = new List<EvaluationSummaryModel>
            {
                new EvaluationSummaryModel(
                    "Increased productivity?",
                    new Dictionary<string, int>
                        { { "Yes", 107 }, { "No", 102 }, { "No response", 7 } }
                ),
                new EvaluationSummaryModel(
                    "Gained new skills?",
                    new Dictionary<string, int>
                        { { "Yes", 102 }, { "No", 107 }, { "No response", 7 } }
                ),
                new EvaluationSummaryModel(
                    "Perform faster?",
                    new Dictionary<string, int>
                        { { "Yes", 112 }, { "No", 95 }, { "No response", 9 } }
                ),
                new EvaluationSummaryModel(
                    "Time saving per week",
                    new Dictionary<string, int>
                    {
                        { "0 hrs", 95 }, { "<1 hr", 54 }, { "1-2 hrs", 30 },
                        { "2-4 hrs", 16 }, { "4-6 hrs", 7 }, { ">6 hrs", 3 },
                        { "No response", 11 }
                    }
                ),
                new EvaluationSummaryModel(
                    "Pass on skills?",
                    new Dictionary<string, int>
                        { { "Yes", 136 }, { "No", 64 }, { "No response", 16 } }
                ),
                new EvaluationSummaryModel(
                    "Help with patients/clients?",
                    new Dictionary<string, int>
                    {
                        { "Yes, directly", 34 }, { "Yes, indirectly", 86 },
                        { "No", 64 }, { "No response", 9 }
                    }
                ),
                new EvaluationSummaryModel(
                    "Recommended materials?",
                    new Dictionary<string, int>
                        { { "Yes", 157 }, { "No", 51 }, { "No response", 8 } }
                )
            };

            // When
            var result = evaluationSummaryService.GetEvaluationSummaryModels(centreId, activityFilterData);

            // Then
            result.Should().BeEquivalentTo(expectedResults);
        }
    }
}
