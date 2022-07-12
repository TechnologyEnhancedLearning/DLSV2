namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.IO;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryServiceTests
    {
        public const string EvaluationSummaryDownloadRelativeFilePath =
            "\\TestData\\EvaluationSummaryDownloadTest.xlsx";

        private IEvaluationSummaryDataService evaluationSummaryDataService = null!;
        private IEvaluationSummaryService evaluationSummaryService = null!;

        [SetUp]
        public void SetUp()
        {
            evaluationSummaryDataService = A.Fake<IEvaluationSummaryDataService>();
            evaluationSummaryService = new EvaluationSummaryService(evaluationSummaryDataService);
        }

        [Test]
        public void GetEvaluationResponseBreakdowns_returns_list_of_models_correctly()
        {
            // Given
            const int centreId = 121;
            var data = EvaluationSummaryTestHelper.GetDefaultEvaluationAnswerCounts();
            var expectedResults = EvaluationSummaryTestHelper.GetDefaultEvaluationResponseBreakdowns();
            var activityFilterData = new ActivityFilterData(
                DateTime.Today,
                DateTime.Today,
                null,
                null,
                null,
                CourseFilterType.None,
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
            var result = evaluationSummaryService.GetEvaluationSummary(centreId, activityFilterData);

            // Then
            result.Should().BeEquivalentTo(expectedResults);
        }

        [Test]
        public void GetEvaluationSummaryFileForCentre_returns_expected_excel_data()
        {
            // Given
            using var expectedWorkbook = new XLWorkbook(
                TestContext.CurrentContext.TestDirectory + EvaluationSummaryDownloadRelativeFilePath
            );
            GivenEvaluationSummaryDataServiceReturnsDataInExampleSheet();

            var filterData = new ActivityFilterData(
                DateTime.Parse("2020-9-1"),
                DateTime.Parse("2021-9-1"),
                null,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Months
            );

            // When
            var resultBytes = evaluationSummaryService.GetEvaluationSummaryFileForCentre(101, filterData);

            using var resultsStream = new MemoryStream(resultBytes);
            using var resultWorkbook = new XLWorkbook(resultsStream);

            // Then
            SpreadsheetTestHelper.AssertSpreadsheetsAreEquivalent(expectedWorkbook, resultWorkbook);
        }

        private void GivenEvaluationSummaryDataServiceReturnsDataInExampleSheet()
        {
            var returnedData = new EvaluationAnswerCounts
            {
                Q1Yes = 174,
                Q1No = 69,
                Q1NoResponse = 32,
                Q2Yes = 170,
                Q2No = 70,
                Q2NoResponse = 35,
                Q3Yes = 167,
                Q3No = 75,
                Q3NoResponse = 33,
                Q4Hrs0 = 75,
                Q4HrsLt1 = 106,
                Q4Hrs1To2 = 35,
                Q4Hrs2To4 = 13,
                Q4Hrs4To6 = 6,
                Q4HrsGt6 = 3,
                Q4NoResponse = 37,
                Q5Yes = 178,
                Q5No = 58,
                Q5NoResponse = 39,
                Q6YesDirectly = 8,
                Q6YesIndirectly = 119,
                Q6No = 72,
                Q6NoResponse = 34,
                Q6NotApplicable = 0,
                Q7Yes = 188,
                Q7No = 53,
                Q7NoResponse = 34
            };

            A.CallTo(
                () => evaluationSummaryDataService.GetEvaluationSummaryData(
                    A<int>._,
                    A<DateTime>._,
                    A<DateTime>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns(returnedData);
        }
    }
}
