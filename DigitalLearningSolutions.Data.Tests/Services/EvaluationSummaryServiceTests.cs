namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
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
            const int centreId = 121;
            var data = EvaluationSummaryTestHelper.GetDefaultEvaluationAnswerCounts();
            var expectedResults = EvaluationSummaryTestHelper.GetDefaultEvaluationSummaryModels();
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

            // When
            var result = evaluationSummaryService.GetEvaluationSummaryModels(centreId, activityFilterData);

            // Then
            result.Should().BeEquivalentTo(expectedResults);
        }
    }
}
