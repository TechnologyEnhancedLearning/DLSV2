namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class TrackerActionServiceTests
    {
        private const int DefaultProgressId = 101;
        private const int DefaultCustomisationVersion = 1;
        private const string? DefaultLmGvSectionRow = "Test";
        private const int DefaultTutorialId = 123;
        private const int DefaultTutorialTime = 2;
        private const int DefaultTutorialStatus = 3;
        private const int DefaultDelegateId = 4;
        private const int DefaultCustomisationId = 5;
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
            const string diagnosticOutcome = "[{'tutorialid':425,'myscore':4},{'tutorialid':424,'myscore':3}]";

            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).DoesNothing();

            // When
            var result = trackerActionService.StoreDiagnosticJson(DefaultProgressId, diagnosticOutcome);

            // Then
            result.Should().Be(TrackerEndpointResponse.Success);
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    DefaultProgressId,
                    424,
                    3
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.UpdateDiagnosticScore(
                    DefaultProgressId,
                    425,
                    4
                )
            ).MustHaveHappenedOnceExactly();
        }

        [TestCase(1, null)]
        [TestCase(null, "[{'tutorialid':425,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'unexpectedkey':425,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':999999999999999999,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':x,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':425,'myscore':x},{'tutorialid':424,'myscore':3}]")]
        [TestCase(1, "[{'tutorialid':0,'myscore':4},{'tutorialid':424,'myscore':3}]")]
        public void
            StoreDiagnosticJson_returns_StoreDiagnosticScoreException_if_error_when_deserializing_json_or_updating_score(
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

        [Test]
        public void StoreAspProgressV2_returns_success_response_if_successful()
        {
            // Given
            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = trackerActionService.StoreAspProgressV2(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.Success);
            A.CallTo(
                () => progressService.GetDetailedCourseProgress(
                    DefaultProgressId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.StoreAspProgressV2(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultLmGvSectionRow,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(A<DetailedCourseProgress>._)
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreAspProgressV2_calls_CheckProgressForCompletion_if_TutorialStatus_is_2()
        {
            // Given
            const int tutorialStatus = 2;

            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = trackerActionService.StoreAspProgressV2(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                tutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.Success);
            A.CallTo(
                () => progressService.CheckProgressForCompletionAndSendEmailIfCompleted(
                    A<DetailedCourseProgress>.That.Matches(
                        progress => progress.ProgressId == DefaultProgressId &&
                                    progress.DelegateId == DefaultDelegateId &&
                                    progress.CustomisationId == DefaultCustomisationId
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void StoreAspProgressV2_returns_StoreAspProgressV2Exception_if_progress_is_null()
        {
            // Given
            A.CallTo(
                () => progressService.GetDetailedCourseProgress(DefaultProgressId)
            ).Returns(null);

            // When
            var result = trackerActionService.StoreAspProgressV2(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressV2Exception);
            A.CallTo(
                () => progressService.StoreAspProgressV2(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [TestCase(100, DefaultCustomisationId)]
        [TestCase(DefaultDelegateId, 100)]
        public void
            StoreAspProgressV2_returns_StoreAspProgressV2Exception_if_progress_is_null_or_a_param_does_not_match_progress_record(
                int delegateId,
                int customisationId
            )
        {
            // Given
            ProgressServiceReturnsDefaultDetailedCourseProgress();

            // When
            var result = trackerActionService.StoreAspProgressV2(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                delegateId!,
                customisationId
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressV2Exception);
            A.CallTo(
                () => progressService.StoreAspProgressV2(
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        private void ProgressServiceReturnsDefaultDetailedCourseProgress()
        {
            var detailedCourseProgress =
                ProgressTestHelper.GetDefaultDetailedCourseProgress(
                    DefaultProgressId,
                    DefaultDelegateId,
                    DefaultCustomisationId
                );
            A.CallTo(
                () => progressService.GetDetailedCourseProgress(DefaultProgressId)
            ).Returns(detailedCourseProgress);
        }
    }
}
