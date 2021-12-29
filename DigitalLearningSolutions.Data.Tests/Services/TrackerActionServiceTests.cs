namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class TrackerActionServiceTests
    {
        private ITutorialContentDataService dataService = null!;
        private ILogger<TrackerActionService> logger = null!;
        private IProgressService progressService = null!;
        private ITrackerActionService trackerActionService = null!;

        [SetUp]
        public void Setup()
        {
            dataService = A.Fake<ITutorialContentDataService>();
            progressService = A.Fake<IProgressService>();
            logger = A.Fake<ILogger<TrackerActionService>>();

            trackerActionService = new TrackerActionService(dataService, progressService, logger);
        }

        [Test]
        public void GetObjectiveArray_returns_results_in_specified_json_format()
        {
            // Given
            var sampleObjectiveArrayResult = new[]
            {
                new Objective(1, new List<int> { 6, 7, 8 }, 4),
                new Objective(2, new List<int> { 17, 18, 19 }, 0),
            };
            A.CallTo(() => dataService.GetNonArchivedObjectivesBySectionAndCustomisationId(A<int>._, A<int>._))
                .Returns(sampleObjectiveArrayResult);

            // When
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // Then
            result.Should().BeEquivalentTo(new TrackerObjectiveArray(sampleObjectiveArrayResult));
        }

        [Test]
        public void GetObjectiveArray_returns_empty_object_json_if_no_results_found()
        {
            // Given
            A.CallTo(() => dataService.GetNonArchivedObjectivesBySectionAndCustomisationId(A<int>._, A<int>._))
                .Returns(new List<Objective>());

            // When
            var result = trackerActionService.GetObjectiveArray(1, 1);

            // Then
            result.Should().Be(null);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, 1)]
        public void GetObjectiveArray_returns_null_if_parameter_missing(
            int? customisationId,
            int? sectionId
        )
        {
            // Given
            A.CallTo(() => dataService.GetNonArchivedObjectivesBySectionAndCustomisationId(A<int>._, A<int>._))
                .Returns(new[] { new Objective(1, new List<int> { 1 }, 9) });

            // When
            var result = trackerActionService.GetObjectiveArray(customisationId, sectionId);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void GetObjectiveArrayCc_returns_results_in_specified_json_format()
        {
            // Given
            var sampleCcObjectiveArrayResult = new[]
            {
                new CcObjective(1, "name1", 4),
                new CcObjective(1, "name2", 0),
            };
            A.CallTo(() => dataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(1, 1, true))
                .Returns(sampleCcObjectiveArrayResult);

            // When
            var result = trackerActionService.GetObjectiveArrayCc(1, 1, true);

            // Then
            result.Should().BeEquivalentTo(new TrackerObjectiveArrayCc(sampleCcObjectiveArrayResult));
        }

        [Test]
        public void GetObjectiveArrayCc_returns_empty_object_json_if_no_results_found()
        {
            // Given
            A.CallTo(
                    () => dataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(
                        A<int>._,
                        A<int>._,
                        A<bool>._
                    )
                )
                .Returns(new List<CcObjective>());

            // When
            var result = trackerActionService.GetObjectiveArrayCc(1, 1, true);

            // Then
            result.Should().Be(null);
        }

        [Test]
        [TestCase(null, 1, true)]
        [TestCase(1, null, true)]
        [TestCase(1, 1, null)]
        public void GetObjectiveArrayCc_returns_null_if_parameter_missing(
            int? customisationId,
            int? sectionId,
            bool? isPostLearning
        )
        {
            // When
            var result = trackerActionService.GetObjectiveArrayCc(customisationId, sectionId, isPostLearning);

            // Then
            result.Should().Be(null);
            A.CallTo(
                () => dataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreDiagnosticJson_returns_success_response_if_successful()
        {
            // Given
            const int progressId = 1;
            const string diagnosticOutcome = "[{'tutorialid':425,'myscore':4},{'tutorialid':424,'myscore':3}]";

            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).DoesNothing();

            // When
            var result = trackerActionService.StoreDiagnosticJson(progressId, diagnosticOutcome);

            // Then
            result.Should().Be(TrackerEndpointResponse.Success);
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    1,
                    424,
                    3
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    1,
                    425,
                    4
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(null, "[{'tutorialid':425,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, null)]
        public void StoreDiagnosticJson_returns_StoreDiagnosticScoreException_if_parameter_missing(
            int? progressId,
            string? diagnosticOutcome
        )
        {
            // When
            var result = trackerActionService.StoreDiagnosticJson(progressId, diagnosticOutcome);

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreDiagnosticScoreException);
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }
    }
}
