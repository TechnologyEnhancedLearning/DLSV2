namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryDataServiceTests
    {
        private IEvaluationSummaryDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new EvaluationSummaryDataService(connection);
        }

        [Test]
        public void GetEvaluationSummaryData_fetches_data_correctly()
        {
            // Given
            var expectedResult = EvaluationSummaryTestHelper.GetDefaultEvaluationAnswerCounts();

            // When
            var result = service.GetEvaluationSummaryData(
                121,
                new DateTime(2010, 01, 01),
                new DateTime(2020, 01, 01),
                1,
                3,
                10059
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
