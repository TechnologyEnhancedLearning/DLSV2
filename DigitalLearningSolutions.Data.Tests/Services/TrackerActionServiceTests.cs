namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
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
        private const int DefaultSessionId = 312;

        private readonly DetailedCourseProgress detailedCourseProgress =
            ProgressTestHelper.GetDefaultDetailedCourseProgress(
                DefaultProgressId,
                DefaultDelegateId,
                DefaultCustomisationId
            );

        private ITutorialContentDataService dataService = null!;
        private ILogger<TrackerActionService> logger = null!;
        private IProgressService progressService = null!;
        private ISessionDataService sessionDataService = null!;
        private IStoreAspProgressService storeAspProgressService = null!;
        private ITrackerActionService trackerActionService = null!;

        [SetUp]
        public void Setup()
        {
            dataService = A.Fake<ITutorialContentDataService>();
            progressService = A.Fake<IProgressService>();
            sessionDataService = A.Fake<ISessionDataService>();
            storeAspProgressService = A.Fake<IStoreAspProgressService>();
            logger = A.Fake<ILogger<TrackerActionService>>();

            trackerActionService = new TrackerActionService(
                dataService,
                progressService,
                sessionDataService,
                storeAspProgressService,
                logger
            );
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
        public void StoreAspProgressV2_returns_non_null_exceptions_from_validation()
        {
            // Given
            A.CallTo(
                () => storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspProgressException, null));

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
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
            A.CallTo(
                () => storeAspProgressService.StoreAspProgressAndSendEmailIfComplete(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreAspProgressV2_stores_progress_when_valid_and_returns_success_response_if_successful()
        {
            // Given
            StoreAspProgressServiceReturnsDefaultDetailedCourseProgressOnValidation();

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
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                CallsToStoreAspProgressV2MethodsMustHaveHappened();
            }
        }

        [Test]
        public void StoreAspProgressNoSession_returns_non_null_exceptions_from_query_and_progress_validation()
        {
            // Given
            A.CallTo(
                () => storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._,
                    A<int?>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspProgressException, null));

            // When
            var result = trackerActionService.StoreAspProgressNoSession(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
            A.CallTo(
                () => storeAspProgressService.StoreAspProgressAndSendEmailIfComplete(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void StoreAspProgressNoSession_returns_non_null_exceptions_from_session_validation()
        {
            // Given
            StoreAspProgressServiceReturnsDefaultDetailedCourseProgressOnValidation();
            A.CallTo(
                () => storeAspProgressService.ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                    A<string?>._,
                    A<int>._,
                    A<int>._
                )
            ).Returns((TrackerEndpointResponse.StoreAspProgressException, null));

            // When
            var result = trackerActionService.StoreAspProgressNoSession(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            result.Should().Be(TrackerEndpointResponse.StoreAspProgressException);
            A.CallTo(
                () => storeAspProgressService.StoreAspProgressAndSendEmailIfComplete(
                    A<DetailedCourseProgress>._,
                    A<int>._,
                    A<string?>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public void
            StoreAspProgressNoSession_updates_learning_time_and_stores_progress_when_valid_and_returns_success_response_if_successful()
        {
            // Given
            StoreAspProgressServiceReturnsDefaultDetailedCourseProgressOnValidation();
            StoreAspProgressServiceReturnsDefaultSessionOnValidation();

            // When
            var result = trackerActionService.StoreAspProgressNoSession(
                DefaultProgressId,
                DefaultCustomisationVersion,
                DefaultLmGvSectionRow,
                DefaultTutorialId,
                DefaultTutorialTime,
                DefaultTutorialStatus,
                DefaultDelegateId,
                DefaultCustomisationId,
                DefaultSessionId.ToString()
            );

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(TrackerEndpointResponse.Success);
                A.CallTo(
                    () => sessionDataService.AddTutorialTimeToSessionDuration(DefaultSessionId, DefaultTutorialTime)
                ).MustHaveHappenedOnceExactly();
                CallsToStoreAspProgressV2MethodsMustHaveHappened();
            }
        }

        private void StoreAspProgressServiceReturnsDefaultDetailedCourseProgressOnValidation()
        {
            A.CallTo(
                () => storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus,
                    DefaultDelegateId,
                    DefaultCustomisationId
                )
            ).Returns((null, detailedCourseProgress));
        }

        private void StoreAspProgressServiceReturnsDefaultSessionOnValidation()
        {
            A.CallTo(
                () => storeAspProgressService.ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                    DefaultSessionId.ToString(),
                    DefaultDelegateId,
                    DefaultCustomisationId
                )
            ).Returns((null, DefaultSessionId));
        }

        private void CallsToStoreAspProgressV2MethodsMustHaveHappened()
        {
            A.CallTo(
                () => storeAspProgressService.GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                    DefaultProgressId,
                    DefaultCustomisationVersion,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus,
                    DefaultDelegateId,
                    DefaultCustomisationId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => storeAspProgressService.StoreAspProgressAndSendEmailIfComplete(
                    detailedCourseProgress,
                    DefaultCustomisationVersion,
                    DefaultLmGvSectionRow,
                    DefaultTutorialId,
                    DefaultTutorialTime,
                    DefaultTutorialStatus
                )
            ).MustHaveHappenedOnceExactly();
        }
    }
}
